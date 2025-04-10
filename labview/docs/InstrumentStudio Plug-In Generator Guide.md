# InstrumentStudio Plug-In Generator for LabVIEW

- [InstrumentStudio Plug-In Generator for LabVIEW](#instrumentstudio-plug-in-generator-for-labview)
  - [Introduction](#introduction)
  - [Workflow](#workflow)
  - [Software support](#software-support)
  - [Installation](#installation)
  - [Developing a LabVIEW InstrumentStudio plug-in](#developing-a-labview-instrumentstudio-plug-in)
  - [Deploying a LabVIEW InstrumentStudio plug-in](#deploying-a-labview-instrumentstudio-plug-in)
  - [Using a LabVIEW InstrumentStudio plug-in in InstrumentStudio](#using-a-labview-instrumentstudio-plug-in-in-instrumentstudio)
  - [Creating a package or installer to deploy a LabVIEW InstrumentStudio plug-in](#creating-a-package-or-installer-to-deploy-a-labview-instrumentstudio-plug-in)
  - [Note](#note)

---

## Introduction

The InstrumentStudio Plug-In Generator tool allows users to create new plug-ins with a specified
name and group. This tool provides a convenient starting point for developing any LabVIEW
application as an InstrumentStudio plug-in.

---

## Workflow

![User Workflow](./images/InstrumentStudio%20Plug-In%20Generator%20Guide/Workflow.png)

---

## Software support

NI Packages Dependency | Version Required
--- | ---
[LabVIEW (64-bit)](https://www.ni.com/en/support/downloads/software-products/download.labview.html#443865) | 2021 SP1 or later
[InstrumentStudio](https://www.ni.com/en/support/downloads/software-products/download.instrumentstudio.html#460631) | 2022 Q3 or later
[JKI VI Package Manager](https://www.ni.com/en/support/downloads/tools-network/download.jki-vi-package-manager.html#443251) | 2021 SP1 or later

---

## Installation

Download and install the `InstrumentStudio Plug-In SDK` and `InstrumentStudio Plug-In Generator`
packages from the latest release assets.

> [!NOTE]  
> Please install the `InstrumentStudio Plug-In SDK` package before installing the `InstrumentStudio
> Plug-In Generator` package.

---

## Developing a LabVIEW InstrumentStudio plug-in

1. Create and save a new LabVIEW project.
2. From the project window, go to `Tools` → `Plug-In SDKs` → `InstrumentStudio Plug-In` → `Create
   InstrumentStudio Plug-in...`.
   1. This will open the `Create InstrumentStudio Plug-in` dialog.
   2. The name of the active LabVIEW project will be automatically populated in the `Active Project
      Name` indicator.  
      - The new InstrumentStudio plug-in will be created in this active LabVIEW project.
   3. Enter the `InstrumentStudio Plug-in Name` and `InstrumentStudio Plug-in Group`.
      - The `InstrumentStudio Plug-in Group` input is optional and will be set to default value
        'Default' if not provided.
   4. Click on the `Create InstrumentStudio Plug-in` button.

      ![Create InstrumentStudio Plug-In](./images/InstrumentStudio%20Plug-In%20Generator%20Guide/Create%20InstrumentStudio%20Plug-In.png)

   5. This will create a new InstrumentStudio plug-in library and a PPL build specification for the
      plug-in in the active LabVIEW project.

      ![InstrumentStudio Plug-In Library](./images/InstrumentStudio%20Plug-In%20Generator%20Guide/InstrumentStudio%20Plug-In%20Library.png)

3. By default, the top-level plug-in VI contains the template logic that demonstrates the basic
   features of LabVIEW InstrumentStudio plug-ins.

   ![Top-level plug-in VI Block diagram](./images/InstrumentStudio%20Plug-In%20Generator%20Guide/Top-level%20VI%20Block%20diagram.png)

4. Add your logic implementation to the top-level plug-in VI.
      - For reference, please refer to the example plug-ins in the repository.

> [!NOTE]  
> For more information about the GPluginData file format and InstrumentStudio plug-in development,
> please refer to the
> [GPluginData-File-Format](https://github.com/ni/instrumentstudio-plugins/blob/main/labview/docs/GPluginData-File-Format.pdf)
> and [G Plugin Development
> Guide](https://github.com/ni/instrumentstudio-plugins/blob/main/labview/docs/G%20Plugin%20Development%20Guide.pdf)
> documents.

---

## Deploying a LabVIEW InstrumentStudio plug-in

1. Open the active LabVIEW project where the InstrumentStudio plug-in was created.
2. The generated plug-in comes with a Packed Project Library (PPL) build specification.
3. To build the PPL, right-click on the PPL build specification and choose 'Build'.
4. Copy or install the built plug-in files into the InstrumentStudio `Addons` directory, which is
   `C:\Program Files\National Instruments\InstrumentStudio\Addons` by default. This action will
   require administrative permission.
   1. You may optionally install the plug-in files into a subdirectory of the `Addons` directory so
      its contents do not conflict with other plug-ins.
   2. In addition to the default `Addons` directory, you can also install the plug-ins to a
      user-defined directory configured in the InstrumentStudio preferences dialog for plug-ins.
      - This user-defined directory allows for the installation of plug-ins without requiring
        administrative permissions.
      - To configure this preference, InstrumentStudio must be launched with administrator
        privileges.

      ![Preferences for Plugins dialog](./images/InstrumentStudio%20Plug-In%20Generator%20Guide/Preferences%20for%20Plugins.png)

> [!NOTE]  
> For information about the recommended PPL build settings for an InstrumentStudio plug-in, please
> refer to the 'Building and Deploying Release Plug-Ins' section in the [G Plugin Development
> Guide](https://github.com/ni/instrumentstudio-plugins/blob/main/labview/docs/G%20Plugin%20Development%20Guide.pdf)
> document.

---

## Using a LabVIEW InstrumentStudio plug-in in InstrumentStudio

1. Open InstrumentStudio and click `Manual Layout` to open the Edit Layout dialog.
2. The InstrumentStudio plug-in will be listed under the Add-Ons category within the group specified
   at the time of plug-in creation.  
   1. If `InstrumentStudio Plug-in Group` was not specified during plug-in creation, the plug-in
      will be populated under the `Default` group.
   2. The `InstrumentStudio Plug-in Group` specified at the time of plug-in creation is saved in the
      plug-in's `.gplugindata` file, which holds the properties of the plug-in.
      - If needed, the user can edit the `GroupName` property in the file to modify the plug-in's
        group later.
3. Choose the desired plug-in and create a large panel.
  
   ![Edit Layout](./images/InstrumentStudio%20Plug-In%20Generator%20Guide/InstrumentStudio%20Edit%20Layout.png)

4. The layout will be populated with the plug-in UI as shown below.

   ![Plug-In SFP](./images/InstrumentStudio%20Plug-In%20Generator%20Guide/InstrumentStudio%20Plug-In%20Soft%20Panel.png)

5. Modify the inputs in the UI to interact with the application.

---

## Creating a package or installer to deploy a LabVIEW InstrumentStudio plug-in

1. Open the active LabVIEW project where the InstrumentStudio plug-in was created.
2. Create a Package or Installer build specification in the project. Refer to
   [Creating Build Specifications](https://www.ni.com/docs/en-US/bundle/labview/page/creating-build-specifications.html)
   for more information.

   ![Package or Installer build spec creation](./images/InstrumentStudio%20Plug-In%20Generator%20Guide/Package%20or%20Installer%20build%20spec.png)

3. Open the package or installer build specification properties, navigate to the `Destinations` page
   in the properties.
4. Set the destination directory to `C:\Program Files\National
   Instruments\InstrumentStudio\Addons\<Sub-directory name>`.

   ![Destination directory](./images/InstrumentStudio%20Plug-In%20Generator%20Guide/Package%20or%20Installer%20Destination%20directory.png)

5. In the `Source Files` page, add the `PPL` build specification of the plug-in to the destination
   view.
   - This action will automatically include the files generated by the PPL into the package or
     installer during the build process.

   ![Source Files](./images/InstrumentStudio%20Plug-In%20Generator%20Guide/Package%20or%20Installer%20Source%20Files.png)

6. To build the package or installer, right-click on the desired build specification and choose
   'Build'.

> [!NOTE]  
> The PPL build specification must be built first in order for the package or installer build to
> succeed.

---

## Note

For information about InstrumentStudio Plug-In SDK APIs, please refer to the
[InstrumentStudio Plugin SDK Reference](https://github.com/ni/instrumentstudio-plugins/blob/main/labview/docs/InstrumentStudio%20Plugin%20SDK%20Reference.pdf)
document.
