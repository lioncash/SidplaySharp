#include <stdint.h>
#include "sidplay2.h"
#include "../../src/player.h"
#include "../../../builders/resid-builder/include/sidplay/builders/resid.h"

// Defines for shortening some typing.
#define Export __declspec(dllexport)
typedef sid2_config_t SidConfig;
typedef sid2_clock_t  SidClock;
typedef sid2_model_t  SidModel;
typedef uint8_t       byte;
typedef uint32_t      uint;

extern "C"
{
    Export int InitializePlayer(byte* fileBytes, int fileLength, int desiredSampleRate, int channels, SidModel sidToUse, SidClock clockFreqToUse);
    Export void FreePlayer();
    Export int GenerateSamples(byte* buffer, int sizeOfBuffer);
    Export bool SetTune(int tune);
    Export char* TuneTitle();
    Export char* TuneAuthor();
    Export char* TuneCopyright();
    Export int NumberOfSubtunes();
    Export int StartingTune();
    Export int CurrentSong();
}


static int usingNTSC       = false; // TODO: Make configurable.
static bool using6581      = false; // TODO: Make configurable.
static bool overrideConfig = false; // TODO: Make configurable.

// Struct which represents the player.
struct Player
{
    SidTune*      sidTune;
    ReSIDBuilder* sidBuilder;
    sidplay2*     sidEmu;
    SidTuneInfo   sidInfo;
};

Player* playerInstance;

// Loads the given bytes of a file into a new player.
//
// Params:
//
// fileBytes         : The file as bytes.
// byteLength        : Length of the file bytes.
// desiredSampleRate : The desired sample rate to use.
// channels          : The number of channels to use.
// sidToUse          : The type of SID chip to use.
// clockFreqToUse   : The clock speed to use for playback.
Export int InitializePlayer(byte* fileBytes, int fileLength, int desiredSampleRate, int channels, SidModel sidToUse, SidClock clockFreqToUse)
{
    Player* player  = new Player();
    player->sidTune = new SidTune(fileBytes, fileLength);

    // If we get an error message from trying to load the file.
    // then return that we got an error.
    if (!player->sidTune->getStatus())
    {
        delete player->sidTune;
        delete player;
        return -1;
    }

    // Check other parameters
    if (desiredSampleRate <= 0 || channels <= 0)
    {
        delete player->sidTune;
        delete player;
        return -2;
    }

    // Otherwise, we continue loading.
    player->sidTune->selectSong(0);            // 0 is the first song index of a SidTune, so always default to this first.
    player->sidTune->getInfo(player->sidInfo); // Load the info of the SidTune into the SidTuneInfo instance we have.

    // Make a reference so we can read the SidTune info here.
    SidTuneInfo& info = player->sidInfo;

    // Initialize a new sidplayer2 emulation instance.
    player->sidEmu = new sidplay2;

    // Create a new config instance for the emulation.
    SidConfig config     = player->sidEmu->config();
    config.clockForced   = false;
    config.clockSpeed    = clockFreqToUse;
    config.clockDefault  = clockFreqToUse;//(usingNTSC & 1) ? SID2_CLOCK_NTSC : SID2_CLOCK_PAL;
    config.frequency     = desiredSampleRate;
    config.playback      = (channels != 1) ? sid2_stereo : sid2_mono;
    config.precision     = 16;

    // Determing the SID model to use.
    config.sidModel      = sidToUse;
    config.sidDefault    = sidToUse;
    config.sidSamples    = true;
    config.environment   = sid2_envR;
    config.forceDualSids = false; // TODO: Make configurable.
    config.emulateStereo = false; // TODO: Make configurable.
    config.optimisation  = SID2_MAX_OPTIMISATION;

    // Create a new ReSID instance.
    player->sidBuilder   = new ReSIDBuilder("resid");
    config.sidEmulation  = player->sidBuilder;
    player->sidBuilder->create(1);    // Create one SID.
    player->sidBuilder->filter(true); // Use a filter. Could possibly make this configurable (though it'd sound crappy without it).

    // Verify everything up to this point (by trying to load the tune with the set config).
    int returnCode = player->sidEmu->load(player->sidTune);
    if (returnCode != 0)
    {
        delete player->sidEmu;
        delete player->sidBuilder;
        delete player->sidTune;
        delete player;

        return -3;
    }

    // Seems alright, lets continue.

    // Set the config to use.
    player->sidEmu->config(config);
    playerInstance = player;
    return 0;
}

// Deallocates the player.
Export void FreePlayer()
{
    delete playerInstance->sidEmu;
    delete playerInstance->sidBuilder;
    delete playerInstance->sidTune;
    delete playerInstance;
}

// Generates the given number of samples into the given buffer.
Export int GenerateSamples(byte* buffer, int samplesToGenerate)
{
    if (playerInstance)
    {
        int numSamplesGenerated = playerInstance->sidEmu->play(buffer, samplesToGenerate);

        return numSamplesGenerated;
    }
    else // Uninitialized player.
    {
        return -1;
    }
}

// Set a tune to play.
// Note that tunes in SidTune objects are 0-based.
// IE) The first tune is index 0, etc.
Export bool SetTune(int tune)
{
    if (playerInstance)
    {
        playerInstance->sidTune->selectSong(tune);
        playerInstance->sidEmu->load(playerInstance->sidTune);

        return true;
    }
    else // Uninitialized player.
    {
        return false;
    }
}

// Information functions.

// Title of the tune.
Export char* TuneTitle()
{
    if (playerInstance)
    {
        return playerInstance->sidInfo.infoString[0];
    }
    else
    {
        return nullptr;
    }
}

// Artist of the tune.
Export char* TuneAuthor()
{
    if (playerInstance)
    {
        return playerInstance->sidInfo.infoString[1];
    }
    else
    {
        return nullptr;
    }
}

// Tune copyright.
Export char* TuneCopyright()
{
    if (playerInstance)
    {
        return playerInstance->sidInfo.infoString[2];
    }
    else
    {
        return nullptr;
    }
}

// Number of tunes contained within the SID file.
Export int NumberOfSubtunes()
{
    if (playerInstance)
    {
        return playerInstance->sidInfo.songs;
    }
    else
    {
        return -1;
    }
}

// Starting tune index.
Export int StartingTune()
{
    if (playerInstance)
    {
        return playerInstance->sidInfo.startSong-1;
    }
    else
    {
        return -1;
    }
}

// The current song that's initialized.
Export int CurrentSong()
{
    if (playerInstance)
    {
        return playerInstance->sidInfo.currentSong;
    }
    else
    {
        return -1;
    }
}

