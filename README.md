# Shop Bridge

**Shop Bridge** is a web application that helps track the different items for sale. It is an inventory management system which keeps records of the items with their name, description, price

This project is built in **ASP.NET  (.NET Framework 4.7.2)**


## 📒 Table of Contents 

- [System Requirements](#-system-requirements)
- [Setup](#-setup)
- [Run Project](#-run-project)
- [Usage](#-usage)
- [Run Tests](#-run-tests)
- [Build](#-build)
- [Time Tracking](#-time-tracking)
- [License](#-license)

## ⚙ System Requirements

* IDE Framework - **Visual Studio 2019 or higher**
* Database - **SQL Server 2012 or higher**
* OS - **Windows 8 or higher**
* **IIS** should be installed.
---
## 🛠 Setup

1. Download the project from this repository.
2. Right-click on downloaded zip file. Click Properties. Check the checkbox for **Unblock**. Click Apply.
	> You can skip this step if you are cloning the repository.
	
3. Open **ShopBridge.sln** file via Visual Studio.
4. Right-click on **ShopBridge.Database** and select **Set as Startup Project**.
5. Open **Package Manager Console** in Visual Studio _(**Tools > NuGet Package Manager > Package Manager Console**)_.
	> Make sure Default Project is selected as **ShopBridge.Database** in the Package Manager Console.
6. Copy the below command and paste it in the Package Manager Console window.
    
	`PM > update-database`
	
	> NOTE : After pasting, hit *Enter*.
---
## ⌛ Run Project

* Right-click on **ShopBridge.Web** project. Click _**Set as Startup Project**_.
* Run the project by pressing **F5** in the keyboard.
---
## ✔ Usage

* User can add items in the **Create Inventory Item** section of the page.
* The added items are listed below in the **List Inventory Items** section of the page.
* User can view the **item details** by clicking on each Items.
* Items can also be deleted from the specified **Delete** action button.

---
## 🌐 Build

* In the Build Menu, change Configuration Manager from Debug to **Release**.
* Right-click on **ShopBridge.Web** project. Select **Publish**.
* Select **Folder** from list of Hosting options. Click **Next**.
* Choose a publishing directory. 
* Click **Finish**.
---
## 🕔 Time Tracking

* Backend Functionality - 4 hours




