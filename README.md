# ![Bendy VR](https://github.com/baggyg/BendyVR/blob/main/WebsiteAssets/BendyVRGithub.jpg)

Bendy VR is a VR mod for [Bendy and the Ink Machine](https://store.steampowered.com/app/622650/Bendy_and_the_Ink_Machine/), with full motion control support. This is Team Beef's very first PCVR mod. I would firstly like to thank Raicuparta. Its fair to say that either BendyVR would not have existed without his VR mods or would have certainly taken a long time to create. Quite a few systems are based upon or using  his work, including the Installer. Raicuparta has a slew of his own VR mods which you can browse [on his website](https://raicuparta.com/). 

Secondly I would like to thank all of our patrons for supporting us. Without this support the VR mods we are working on would simply not be possible. If you like what we are doing, want to suggest and vote on new ports and get access to in-development versions please click the banner below

[https://github.com/baggyg/BendyVR/blob/main/WebsiteAssets/reddit_mobile_1280_384.jpg?raw=true](https://www.patreon.com/teambeef)

## How to Install

- [Download the latest release zip](https://github.com/baggyg/BendyVR/releases/latest).
- Extract it somewhere safe (you need to keep this folder for the Mod to function). 
- Open 'TeamBeefInstaller.Exe' and click Install
- Either Run the game as usual or by clicking the "Start Game" button in the Installer.

## Requirements

- A compatible version of Bendy and the Ink Machine. Currently that's version 1.1.2. This version is available in these stores:
  - [Steam](https://store.steampowered.com/app/622650/Bendy_and_the_Ink_Machine/)  
- A PC ready for PCVR. Bendy VR doesn't work on standalone VR platforms.
- An OpenVR-compatible VR headset. Examples:
  - Quest 2 connected to the PC via Link Cable, Air Link, Virtual Desktop, ALVR, etc
  - Any Oculus Rift
  - Valve Index
  - Any Vive
  - Any Windows Mixed Reality device (probably?)
- VR controllers. This isn't playable with a normal game controller, motion controls are required.
- Steam and SteamVR installed.

## Controls

I've attached a default SteamVR Input for each of the main PCVR headsets. However I was only able to test with Quest 2 (via Link / Air Link). You should be able to see the mapped controls via SteamInput and showing on the controllers. Please let me know if Vive Controllers or Knuckles work and if you have a better default binding, I'd love to have it. 

**Please note that in the menu there are no tracked hands. Use the UiUp / UiDown / Next / Previous / Confirm / Cancel to navigate.**

Sample Oculus Touch Controls (Quest 2 / Rift S controls):

**Non Dominant Hand:**

Joystick = Move

Joystick Click / Grip = Run

Trigger = Interact (What you are interacting with is based on your non-dominant hand. Turn on the Laser sight from the Advanced Menu if you are unsure). 

Y = Pause

**Dominant Hand:**

Joystick Left / Right = Smooth Turn / Snap Turn

B = Seeing Tool (CH5 / New Game+)

A = Jump

Trigger = Attack (Gun / Throw weapon types only)

When you get the axe or other Melee weapon, it is motion controlled so swing to attack. 

## VR Settings

You can find VR Settings in the Advanced Menu (via Main Menu or Pause). 

## Performance 

You want to aim to get a smooth framerate. The game will run best if you turn off all the optional effects (Anti-Aliasing / Depth of Field / Bloom / Ambient Occlusion), so you have quite a few options to get to a reasonable frame rate (in addition to the graphic quality setting and Steam resolution). 

Ambient Occlusion does have some small artifacts which are different in each eye. However all options do add nice graphics to the game. Depth of Field is especially heavy. 

Bendy VR ships with [openvr_fsr](https://github.com/fholger/openvr_fsr), which is already enabled on Ultra Quality Mode. To disable or adjust FSR , edit `\Bendy and the Ink Machine\Bendy and the Ink Machine_Data\Plugins\openvr_mod.cfg`. Check the [openvr_fsr readme](https://github.com/fholger/openvr_fsr#readme) for more details.

## Known Issues

- Ending Cinematic is screen locked - this is due to the Unity Video Player used. 
- After the credits you have to exit game (and restart for new game + / archives). Your game will have been saved.
- You cannot get back to the main menu from the pause menu. Clicking quit and then Confirm will exit the games completely. I will look to fix this in the future. 
- Fog / Some Other Post-Processing Effects rotate with view. Unfortuantely "allowRoll" was added in Unity Version (2018.3) BATIM uses 2018.2. 
- Some of the rising seems to follow your view. This is caused by the original developers using a "Cutout" masking to block of part of the mesh to present the appearnace of "moving ink". I can't decompile this shader so am unable to alter. 

## How to Uninstall

Open the TeamBeefInstaller.exe and click the Uninstall button. 

## Support

If you find bugs or are otherwise facing problems with the mod, please [open an issue](https://github.com/baggyg/BendyVR/issues/new/choose).

You can also contact Team Beef on the [Team Beef Discord](https://discord.gg/fA6m8SMZPA). 
