HoloLens 2 Gait Trainer {#mainpage}
=======================

 \image html hololens2_cover.jpg

### Installing the tools ###

**Install or Update to the latest Windows 10** so that your PC’s operating system matches the platform of your HoloLens 2.

Enable developer mode on your PC at settings > Update & Security > For Developers.

**Remark: Some features in the Mixed Reality environment are not supported on ‘N’ versions of Windows.**

**Install Visual Studio 2019 (16.8 or higher)** with the following features.
    - 
	  Desktop development with C++
	  
	- 
	  Universal Windows Platform (UWP) development
	  
	- 
	  Game development with Unity (if planning to use Unity)
	  
Within the UWP workload, make sure the following components are included for installation:

- Windows 10 SDK version **10.0.19041.0** or **10.0.18362.0**
	  
- USB Device Connectivity (required to deploy/debug to HoloLens over USB)
	  
- **C++ (v142)** Universal Windows Platform tools *(required when using Unity)*
	  
If you already have installed Visual Studio 2019, you can check or add the necessary components by following the steps.

Go to control panel < uninstall a program < select visual studio < change

The above step will open the visual studio installer and from there you can select the individual components tab. From there, by selecting the necessary components you can install the necessary components without reinstalling visual studio.

**Remarks:** There are **some known issues** with debugging mixed reality apps in **Visual Studio 2019 version 16.0**. Please ensure that you update to **Visual Studio 2019 version 16.8 or higher.**
 
**HoloLens 2 Emulator:** If you don’t have a HoloLens or if you need to do quick prototyping,  you can use this emulator. The emulator lets you run applications on a HoloLens virtual machine image without a physical HoloLens.
 
Your system must support **Hyper-V** for the Emulator installation to succeed. Check the system requirement here -> [Emulator]

**Remark:** The emulator is not always accurate.

Unity Hub with **Unity 2019.4.X** or **higher installed**. I am using the **Unity 2021.1.14f1** version for the project.

Once you installed the unity add the following modules. Without these modules, you can’t develop for HoloLens.

- Universal Windows Platform Build Support (known as UWP)
	  
- Windows Build Support(IL2CCP)

 \image html unity_add_module.png

### Mixed Reality Toolkit aka MRTK ###
The Mixed Reality Toolkit (MRTK) is a cross-platform toolkit (**MRTK relies on OpenXR**) for building mixed reality experiences for Virtual Reality (VR) and Augmented Reality (AR). 
The toolkit provides a set of components and features which could be used to accelerate your Windows Mixed Reality development.

Suppose you're creating a mixed reality experience in which the user needs the ability to move, rotate, and scale a holographic object. Although you could start from scratch 
and create your own script to enable such manipulations, your workflow of adding direct manipulation to holograms and configuring constraints may be time-consuming--especially 
if there are multiple objects to configure.

The MRTK has many built-in scripts which you can add to any game object.  The scripts save lots of time because we don’t have to implement lots of things from scratch. 
In addition to that, the MRTK enables rapid prototyping via in-editor simulation that allows you to see changes immediately. Furthermore, it is built in a modular way. 
We only use what we attached to the game objects.  It keeps your project size smaller and makes it easier to manage. Additionally, because it’s built with scriptable 
objects and is interface-driven, it’s also possible for you to replace the components that are included with your own, to support other services, systems, and platforms.

 \image html mrtk.png

Click here to view MRTK documentation -> [MRTK]

### Configure Unity for Windows Mixed Reality ###

Please follow this tutorial to set up a new unity project. -> [Configs]

In the Build Settings window, always make sure that you selected Universal Windows Platform and the following settings:

- Set **Target device** to **HoloLens**
	  
- Set **Architecture** to **ARM64**
	  
- Set **Build Type** to **D3D Project**
	  
- Set **Target SDK Version** to **Latest Installed**
	  
- Set **Minimum Platform Version** to **10.0.1024.0**
	  
- Set **Visual Studio Version** to **Latest installed**
	  
- Set **Build and Run** on to **USB Device**
	  
- Set **Build configuration** to **Release** because there are known performance issues with Debug

- Click the **Switch Platform** button
	  
### Importing MRTK to the Unity ###

MRTK provides four MRTK Unity packages which can be imported into your Unity project. The latest version of the packages can be found in the *Mixed Reality Feature Tool* 
(Download the feature tool from here - [FeatureTool]). The Mixed Reality Toolkit Foundation package is the only required package that must be imported and 
configured to use MRTK with your project. 

**Remark:** In order to run **Mixed Reality Feature Tool**, you need to install **.NET 5.0 runtime.**

Here are step-by-step instructions for importing MRTK to unity. -> [MRTKInstall]

After MRTK is added to the scene and configured, two new objects are added to the Scene hierarchy window:

**MixedRealityToolkit**

**MixedRealityPlayspace**

The MixedRealityToolkit object contains the toolkit itself. The MixedRealityPlayspace object ensures the headset/controllers and other required systems are managed correctly in the scene.
The Main Camera object is moved as a child to the MixedRealityPlayspace object. This allows the play space to manage the camera simultaneously with the SDKs.

Here is a good source to start development with MRTK. -> [MRTKStart]

I also created a boilerplate with MRTK configurations. You can download it here [Boilerplate]. Please ensure that it has the correct settings.

 \image html openXR.png
 
In both of these locations.
 
 \image html openXR1.png


[Emulator]: https://docs.microsoft.com/en-us/windows/mixed-reality/develop/advanced-concepts/using-the-hololens-emulator
[MRTK]: https://docs.microsoft.com/en-us/windows/mixed-reality/mrtk-unity/?view=mrtkunity-2021-05
[Configs]: https://docs.microsoft.com/en-us/learn/modules/learn-mrtk-tutorials/1-3-exercise-configure-unity-for-windows-mixed-reality
[FeatureTool]: https://www.microsoft.com/en-us/download/details.aspx?id=102778
[MRTKInstall]: https://docs.microsoft.com/en-us/learn/modules/learn-mrtk-tutorials/1-5-exercise-configure-resources
[MRTKStart]: https://docs.microsoft.com/en-us/learn/paths/beginner-hololens-2-tutorials/
[Boilerplate]: https://github.com/PubuduS/MRTK_Boilerplate.git


