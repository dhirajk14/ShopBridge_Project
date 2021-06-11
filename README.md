# Shop Bridge

**Shop Bridge** is a web application that helps track the different items for sale. It is an inventory management system which keeps records of the items with their name, description, price. The project is a Web API developed for the frontend developer to consume to perform ADD , MODIFY , DELETE , LIST functionalities within the database

This project is built in **ASP.NET  (.NET Framework 4.7.2)**


## 📒 Table of Contents 

- [System Requirements](#-system-requirements)
- [Setup](#-setup)
- [Run Project](#-run-project)
- [Usage](#-usage)
- [Build](#-build)
- [Time Tracking](#-time-tracking)

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
	
3. Open **ShopBridge_Project.sln** file via Visual Studio.
4. Right-click on **ShopBridge_Project** and select **Set as Startup Project**.

---
## ⌛ Run Project

* Right-click on **ShopBridge_Project** project. Click _**Set as Startup Project**_.
* Run the project by pressing **F5** in the keyboard.
* Use the localhost url from browser and append the following - api/home/inventory
* URL http://localhost:YOURPORT/api/home/inventory
* Configure your connectionstring in Web Config for DB Connection
* Use Query for DB Table creation ---- CREATE TABLE ShopBridgeProduct (
    ID int IDENTITY(1,1) PRIMARY KEY,
    PRODUCT_NAME varchar(255) NOT NULL,
    PRODUCT_DESCRIPTION varchar(255),
    PRICE varchar(255),
);
ALTER TABLE ShopBridgeProduct ADD IMAGE_FILE NVARCHAR(MAX)

---
## ✔ Usage

* New items can be added using **ADD** key of the API.
* The added items are listed below in the **LISTS** key of the API.
* Item can be modified by using  **MODIFY** key of the API.
* Items can also be deleted from the specified **DELETE** key of the API.

---
## 🌐 Build

* In the Build Menu, change Configuration Manager from Debug to **Release**.
* Right-click on **ShopBridge_Project** project. Select **Publish**.
* Select **Folder** from list of Hosting options. Click **Next**.
* Choose a publishing directory. 
* Click **Finish**.
---
## 🕔 Time Tracking

* Backend Functionality - 4 hours




