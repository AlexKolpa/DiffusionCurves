# DiffusionCurves System
This repo contains the source code to the DiffusionCurves System, our Bachelor Project. Included as well is a pre-packaged installer of the final release.

## Usage
Most libraries have been included, but EmguCV needs to be added separately. This is because of the size of the required dll's. After installing EmguCV, the following EmguCV libraries need to be added to the references:

* Emgu.CV.dll
* Emgu.CV.UI.dll
* Emgu.Util.dll

Besides this, the OpenCV dll's need to be added to the OpenCV folder in the repo. These dll's can be easily obtained by installing EmguCV.
Take care that the build platform of the project is the same as the EmguCV install. Non-matching versions will most likely result in a crashing application.

Finally, if it has not been installed yet, Nunit's VSTestAdapter needs to be added through the Visual Studio Package Manager. More about this process can be found [here](http://nunit.org/index.php?p=vsTestAdapter&r=2.6.2).
