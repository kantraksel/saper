#pragma once
#include <string>
#include <list>

//HKEY redefinition
typedef struct { int unused; } _HRKEY, *HRKEY;

struct Registry
{
	enum RegistryValueType : unsigned long
	{
		RVT_None = 0,
		RVT_STRING = 1,
		RVT_BINARY = 3,
		RVT_DWORD = 4,
		RVT_STRINGTABLE = 7,
		RVT_QWORD = 11,
	};

	static const HRKEY LocalMachineKey;
	static const HRKEY InvalidKey;
	static HRKEY OpenCurrentUserKey();

	static HRKEY CreateKey(HRKEY key, const std::wstring& sPath);
	static HRKEY OpenKey(HRKEY key, const std::wstring& sPath);
	static void CloseKey(HRKEY key);
	static bool RemoveKey(HRKEY key, const std::wstring& sPath);

	static std::wstring ReadString(HRKEY key, const std::wstring& sValue);
	static unsigned int ReadDWORD(HRKEY key, const std::wstring& sValue);
};
