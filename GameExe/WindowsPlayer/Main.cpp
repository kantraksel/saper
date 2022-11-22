#include "PrecompiledHeader.h"
#include "..\UnityPlayerStub\Exports.h"
#include "Registry.h"

#ifdef _WIN64
    #define PLATFROM_ID "x86_64"
    #define STEAM_BITNESS "64"
#elif _WIN32
    #define PLATFROM_ID "x86"
    #define STEAM_BITNESS ""
#endif

// Hint that the discrete gpu should be enabled on optimus/enduro systems
// NVIDIA docs: http://developer.download.nvidia.com/devzone/devcenter/gamegraphics/files/OptimusRenderingPolicies.pdf
// AMD forum post: http://devgurus.amd.com/thread/169965
extern "C"
{
    __declspec(dllexport) DWORD NvOptimusEnablement = 0x00000001;
    __declspec(dllexport) int AmdPowerXpressRequestHighPerformance = 1;
}

std::wstring FromPlayerPrefsName(const std::wstring& name)
{
    //djb2-xor from https://answers.unity.com/questions/177945/playerprefs-changing-the-name-of-keys.html
    unsigned int hash = 5381;
    for (wchar_t c : name)
    {
        hash = hash * 33 ^ c;
    }
    return name + L"_h" + std::to_wstring(hash);
}

bool IsSteamEnabled()
{
    unsigned int value = 0;

    auto cKey = Registry::OpenCurrentUserKey();
    if (cKey != Registry::InvalidKey)
    {
        auto key = Registry::CreateKey(cKey, L"Software\\Kantraksel\\sapergame");
        if (key != Registry::InvalidKey)
        {
            value = Registry::ReadDWORD(key, FromPlayerPrefsName(L"SteamEnabled"));
            Registry::CloseKey(key);
        }
        Registry::CloseKey(cKey);
    }
    
    return value == 1;
}

typedef bool(*SteamAPI_InitFunc)();
void InitSteamIfNecessary()
{
    if (IsSteamEnabled())
    {
        auto hSteam = LoadLibrary(TEXT("Data/Plugins/" PLATFROM_ID "/steam_api" STEAM_BITNESS ".dll"));
        if (hSteam == 0)
        {
            MessageBox(NULL, TEXT("Failed to load steamapi" STEAM_BITNESS ".dll"), TEXT("sapergame"), MB_ICONERROR);
            return;
        }

        auto func = (SteamAPI_InitFunc)GetProcAddress(hSteam, "SteamAPI_Init");
        func();
    }
}

int WINAPI wWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPWSTR lpCmdLine, int nShowCmd)
{
    InitSteamIfNecessary();
    return UnityMain(hInstance, hPrevInstance, lpCmdLine, nShowCmd);
}
