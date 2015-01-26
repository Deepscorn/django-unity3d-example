![Unity_3D_logo.png](https://cloud.githubusercontent.com/assets/9072397/5611600/db8770ac-94c8-11e4-976a-9e42ccf23345.png)
![python-django.png](https://cloud.githubusercontent.com/assets/9072397/5611563/762c7108-94c8-11e4-9d9c-8ae4a703a03e.png)
# Django-Unity-Example #

Django-Unity-Example is an example project of how to use Django as a backend for a Unity3D game. 

### Notes ###
This repository is currently set up using Microsoft Visual Studio and Python Tools for VS, however, you should be able to run this django project on Linux and Mac considered you've got the dependencies(django, restframework). As this is just an example project, this does *not* cover deploying or getting ready for production.

### Goals ###
To have a minimal backend this example project should include the following features:

* Players can signup using an email and password
* Players can login
* Players will be able to save their score and get a list of all scores
* Players can save their game(blob of data) and retreive a list of saved games

### This project will use the following tools and packages ###

* Django v1.7.1
* Django-rest-framework v3.0.2
* Unity3D v4.3+
* MS Visual Studio Community 2013 
* Python tools v2.1 for Visual Studio 

### How do I get set up? ###

**Django**

* Download and install Visual Studio 2013
* Download and install Python 2.7
* Download and install Python Tools for Visual Studio
* Open the solution and either choose or set up a virtual environment(right-click Environments->Add virtual environment), name it 'django-unity'
* Synchronize the database by right-clicking the project->Python->Django Sync DB...
* You will be asked to enter a username and password for the super user, use 'admin' for both username and password


**Unity3D**

* Download and install Unity3D (v4.3 or higher)
* Simply open the Unity project in the Unity editor


### Getting started ###

**Django**
* Start debugging by pressing F5 or clicking the Play button


**Unity3D**
* Either load the main scene which contains the tester or open one of the two example scenes
* Press play


NOTE: THIS EXAMPLE PROJECT IS NOT FINISHED YET

TODO: ADD DISCLAIMER/LICENCE (mention Newtonsofts JSON.NET framework)