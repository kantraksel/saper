#include "Registry.h"
#define WIN32_LEAN_AND_MEAN
#include <Windows.h>

const HRKEY Registry::LocalMachineKey = ((HRKEY)(ULONG_PTR)((LONG)0x80000002));
const HRKEY Registry::InvalidKey = ((HRKEY)(ULONG_PTR)(-1));

HRKEY Registry::OpenCurrentUserKey()
{
	HKEY hKey;
	if (RegOpenCurrentUser(KEY_ALL_ACCESS, &hKey) != ERROR_SUCCESS)
		hKey = (HKEY)InvalidKey;

	return (HRKEY)hKey;
}

HRKEY Registry::CreateKey(HRKEY key, const std::wstring& sPath)
{
	DWORD dwDisp;
	if (RegCreateKeyEx((HKEY)key, sPath.c_str(), 0, nullptr, REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, nullptr, (PHKEY)&key, &dwDisp) != ERROR_SUCCESS)
		key = InvalidKey;

	return key;
}

HRKEY Registry::OpenKey(HRKEY key, const std::wstring& sPath)
{
	if (RegOpenKeyEx((HKEY)key, sPath.c_str(), 0, KEY_ALL_ACCESS, (PHKEY)&key) != ERROR_SUCCESS)
		key = InvalidKey;

	return key;
}

void Registry::CloseKey(HRKEY key)
{
	RegCloseKey((HKEY)key);
}

bool Registry::RemoveKey(HRKEY key, const std::wstring& sPath)
{
	return RegDeleteKey((HKEY)key, sPath.c_str()) == ERROR_SUCCESS;
}

std::wstring Registry::ReadString(HRKEY key, const std::wstring& sValue)
{
	std::wstring sOut;
	HKEY hKey = (HKEY)key;

	wchar_t buffer[MAX_PATH + 1];
	DWORD dwBufSize = MAX_PATH;
	DWORD dwRegsz = REG_SZ;

	if (RegQueryValueEx(hKey, sValue.c_str(), 0, &dwRegsz, (LPBYTE)buffer, &dwBufSize) == ERROR_SUCCESS)
	{
		wchar_t cBuffer;
		for (int i = 0; i < MAX_PATH; ++i)
		{
			cBuffer = buffer[i];
			if (cBuffer == '\0') break;
			sOut += cBuffer;
		}
	}

	return sOut;
}

unsigned int Registry::ReadDWORD(HRKEY key, const std::wstring& sValue)
{
	DWORD dwOut;
	HKEY hKey = (HKEY)key;

	DWORD dwBufSize = sizeof(dwOut);

	auto ret = RegQueryValueEx(hKey, sValue.c_str(), 0, nullptr, (LPBYTE)&dwOut, &dwBufSize);
	if (ret != ERROR_SUCCESS)
		dwOut = 0;

	return dwOut;
}
