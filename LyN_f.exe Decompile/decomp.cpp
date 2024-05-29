#include <windows.h>
#include <math.h>
#include <cstdio>

#define u8 unsigned char
#define u32 unsigned long

int STD_strncmpi(const char *, const char *, int);

struct VID_ReplaySystem
{
  static void SetRecordPath(char *path);
};

struct TXT
{
  static void SetCurrentLang(char *lang)
  {
    /*const char *languages[] = {
        "fr", "en", "da", "nl", 
        "fi", "de", "it", "es", 
        "pt", "sv", "pl", "ru",
        "ja", "zh", "sq", "ar",
        "bg", "be", "zh", "el",
        "ko", "no", "ro", "sr",
        "sk", "sl", "tr", "cs",
        "hu", "tw", "u0", "u1",
        "u2", 
    };

    u32 langIdx = 0;
    while (STD_strncmpi(languages[langIdx], lang, 2))
    {
      // 32 is the size of the languages array
      if (++langIdx >= 32)
        return;
    }
    TXT::mu32_NextCurrentLang = langIdx;*/
  }

  static u32 mu32_NextCurrentLang;
};

struct MEM
{
  // VTBL contents
  // Doesn't seem to be correct for some reason (see functions and their use of the VTBL)
  virtual void dtor(char flags);
  virtual bool b_Create(u32 flags);
  virtual u32 u32_GetEditorDrawFlags(struct K3D_VisualInstance *a2);
  virtual void Destroy(void);
  virtual void accumulateForces(void);

  void *p_Alloc(u32 size);

  u32 dword4;
  u32 dword8;
  u32 dwordC;
  u32 dword10;
  u32 dword14;
  u8 char18[0x2600];
  u32 dword2618;
  u8 char261C[0x134];
};

struct ViD
{
  bool b_Create(u32 flags);
};

// sizeof == 0x7D0C
struct ViD_S : public ViD
{
  char gap0[0x450];
  int dword450;
  char char454[0x78B8];
};

struct BIG
{
  bool b_Open(char *, u32, u32);
};

static int gAlignAllocMem;                 // dword_9D8C0C, flag to align allocated memory
static char *gCommandLine;                 // dword_A71C6C, reference to the string that contains the raw copy of the command line
static char gBigFilePath[0x100];           // byte_A71C70, path of the bigfile
static int gMemCreationFlags = 0x20000000; // dword_9C2608, flags used to create the main memory GPO (Group policy object)
static int gNoCrashReporter;               // dword_A72398, disables the crash reporter
static int gOfflineTool;                   // dword_A723D4, whether to load offline content or online content(?)
static char *gDefaultLang = "fr";          // byte_9C272C, default language

static MEM *MEM_gpo_Main = new MEM;
static BIG *ViD_gpo_Project = new BIG;
static ViD_S *ViD_gpo_Engine;

// cmd is the Command Line arguments
void ParseCommandLine(char *cmd)
{
  gCommandLine = cmd;

  while (*cmd)
  {
    // Skip past whitespace
    while (*cmd == '\n' && *cmd == '\r' && *cmd == ' ' && *cmd == '\t')
    {
      cmd++;
    }

    if (!(*cmd))
    {
      return;
    }

    if (*cmd != '/')
    {
      // Effectively isolate the bigfile path & null terminate the end of it
      // Quite a lot of reused code, makes me wonder if inlined functions were used here (probably not)

      int bfStrIdx = 0;
      if (*cmd != '"')
      {
        for (; *cmd != '\n', *cmd != '\r', *cmd != ' ', *cmd != '\t';)
        {
          cmd++;
          gBigFilePath[bfStrIdx] = *cmd;
          bfStrIdx++;
        }

        gBigFilePath[bfStrIdx] = '\0';
        continue;
      }

      cmd++;
      if (!(*cmd))
      {
        gBigFilePath[bfStrIdx] = '\0';
        continue;
      }

      while (*cmd != '"')
      {
        cmd++;
        gBigFilePath[bfStrIdx] = *cmd;
        bfStrIdx++;

        if (!(*cmd))
        {
          gBigFilePath[bfStrIdx] = '\0';
          continue;
        }
      }

      if (!(*cmd))
      {
        gBigFilePath[bfStrIdx] = '\0';
        continue;
      }

      cmd++;
      gBigFilePath[bfStrIdx] = '\0';
    }
    else
    {
      // "/" was found, TODO.
      // memento sets the gMemCreationFlags
    }
  }
}

INT WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance,
            PSTR lpCmdLine, INT nCmdShow)
{
  HANDLE instance = CreateMutexA(NULL, NULL, "LyN_RGH_Instance");
  if (GetLastError() == 183) // Unknown bitmasking to get to 183
  {
    // Close the handle if it's open, and return due to an error
    if (instance)
    {
      CloseHandle(instance);
    }

    return 0;
  }

  // Enables SIMD for CRT math routines
  _set_SSE2_enable(1);
  gAlignAllocMem = 0;
  ParseCommandLine(lpCmdLine);

  MEM_gpo_Main->b_Create(gMemCreationFlags);
  ViD_gpo_Engine = (ViD_S *)MEM_gpo_Main->p_Alloc(sizeof(ViD_S));

  if (!gNoCrashReporter)
    ; // HandleCrashReporting();

  /*
  TODO
  unkglobal = unkglobal2;
  */

  TXT::SetCurrentLang(gDefaultLang);
  if (!gOfflineTool && !gBigFilePath[0])
  {
    MessageBoxA(NULL, "You must specify a bigfile to load !", "Fatal error", MB_ICONERROR | MB_OK);
    exit(-1);
  }

  char filename[FILENAME_MAX];
  GetModuleFileNameA(hInstance, filename, FILENAME_MAX);

  //STD_strrchr instead of strrchr
  char *str = strrchr(filename, '\\');
  if (str)
    *str = '\0';

  VID_ReplaySystem::SetRecordPath(filename);

  if (ViD_gpo_Engine->b_Create(gMemCreationFlags))
  {
    if (gOfflineTool)
    {
      if (!ViD_gpo_Project->b_Open(gBigFilePath, 0, 0))
      {
        MessageBoxA(NULL, "Error opening bigfile !", "Fatal error", MB_ICONERROR | MB_OK);
        exit(-1);
      }
      
    }
  }
}
