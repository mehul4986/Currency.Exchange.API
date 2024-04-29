
# Currency Exchange Manager

I've developed the Currency Exchange Manager as part of my assessment. 


This assessment has a Currency.Exchange.API solution consisting of 3 APIs.


1 - Convert: This API will convert an amount from a base currency to a target currency.

2 - Conversion History: This will return the list of Convert API results from the database that have been accessed via the 1st API (Convert API).

3 - Conversion API History: This API will fetch the external API's call history from the SQL database.



## Tech Stack

**Framework** .NET 6

**Programming Language** C# 10

**Database Server:** Microsoft SQL Server 2016 SP1 Express Edition

**Cache:** https://redis.io/downloads/

**IDE:** Visual Studio 2022 Community Edition




## Installation

1 - Install the Redis on your machine where you want to run this solution.

First of all you need to download the radis from the microsft archive.
https://github.com/microsoftarchive/redis/releases/tag/win-3.0.504

2 - Then clone the solution on your machine and then build the solution.

3 - Open the SQL Server and run the below script to create the database.

Note - Make sure that you've DB folder exist in your c drive or else create one and then run the script.

```bash
USE [master]
GO
/****** Object:  Database [CurrencyManager]    Script Date: 2024/04/29 22:00:39 ******/
CREATE DATABASE [CurrencyManager]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'CurrencyManager', FILENAME = N'C:\DB\CurrencyManager.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'CurrencyManager_log', FILENAME = N'C:\DB\CurrencyManager_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [CurrencyManager] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [CurrencyManager].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [CurrencyManager] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [CurrencyManager] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [CurrencyManager] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [CurrencyManager] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [CurrencyManager] SET ARITHABORT OFF 
GO
ALTER DATABASE [CurrencyManager] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [CurrencyManager] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [CurrencyManager] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [CurrencyManager] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [CurrencyManager] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [CurrencyManager] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [CurrencyManager] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [CurrencyManager] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [CurrencyManager] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [CurrencyManager] SET  DISABLE_BROKER 
GO
ALTER DATABASE [CurrencyManager] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [CurrencyManager] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [CurrencyManager] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [CurrencyManager] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [CurrencyManager] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [CurrencyManager] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [CurrencyManager] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [CurrencyManager] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [CurrencyManager] SET  MULTI_USER 
GO
ALTER DATABASE [CurrencyManager] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [CurrencyManager] SET DB_CHAINING OFF 
GO
ALTER DATABASE [CurrencyManager] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [CurrencyManager] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [CurrencyManager] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [CurrencyManager] SET QUERY_STORE = OFF
GO
USE [CurrencyManager]
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
USE [CurrencyManager]
GO
/****** Object:  Table [dbo].[ExchangeRate]    Script Date: 2024/04/29 22:00:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExchangeRate](
	[ExchangeId] [int] IDENTITY(1,1) NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
	[BaseCurrency] [varchar](3) NOT NULL,
	[ExchangeRate] [varchar](max) NOT NULL,
 CONSTRAINT [PK_ExchangeRate] PRIMARY KEY CLUSTERED 
(
	[ExchangeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ExchangeRateLog]    Script Date: 2024/04/29 22:00:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExchangeRateLog](
	[ExchangeHistoryId] [int] IDENTITY(1,1) NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
	[BaseCurrency] [varchar](3) NOT NULL,
	[BaseAmount] [decimal](18, 6) NOT NULL,
	[ToCurrency] [varchar](3) NOT NULL,
	[ToAmount] [decimal](18, 6) NOT NULL,
 CONSTRAINT [PK_ExchangeHistory] PRIMARY KEY CLUSTERED 
(
	[ExchangeHistoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[usp_get_conversion_log]    Script Date: 2024/04/29 22:00:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<MP>
-- Create date: <2024-Apr-29>
-- Description:	<It will get the conversion rate>
-- EXEC [dbo].[usp_get_conversion_log] @TimeStamp = '2024-01-01'
-- =============================================
CREATE PROC [dbo].[usp_get_conversion_log]
	@TimeStamp datetime
AS

BEGIN TRY   
	
	select * from ExchangeRateLog where TimeStamp > = @TimeStamp order by TimeStamp desc

END TRY  
BEGIN CATCH  
	THROW;
END CATCH
GO
/****** Object:  StoredProcedure [dbo].[usp_get_conversion_rate]    Script Date: 2024/04/29 22:00:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<MP>
-- Create date: <2024-Apr-25>
-- Description:	<It will get the Exchange Rates history>
-- EXEC [dbo].[usp_get_conversion_rate] @TimeStamp = '2024-01-01'
-- =============================================
CREATE PROC [dbo].[usp_get_conversion_rate]
	@TimeStamp datetime
AS

BEGIN TRY  
	
	select TimeStamp,BaseCurrency as base,ExchangeRate from ExchangeRate where TimeStamp > = @TimeStamp order by 1 desc

END TRY  
BEGIN CATCH  
	THROW;
END CATCH
GO
/****** Object:  StoredProcedure [dbo].[usp_insert_conversion_log]    Script Date: 2024/04/29 22:00:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<MP>
-- Create date: <2024-Apr-25>
-- Description:	<It will insert the the conversion rate>
-- EXEC [dbo].[usp_insert_conversion_log] @TimeStamp = '2024-04-29',@BaseCurrency = 'USD',@BaseAmount = 1.000021,@ToCurrency = 'INR',@ToAmount=84.0200
-- =============================================
CREATE PROCEDURE [dbo].[usp_insert_conversion_log]	
	@TimeStamp datetime,
	@BaseCurrency varchar(3),	
	@BaseAmount decimal(18,6),
	@ToCurrency varchar(3),
	@ToAmount decimal(18,6)
AS
BEGIN TRY   

	INSERT INTO ExchangeRateLog
	(TimeStamp, BaseCurrency, BaseAmount, ToCurrency, ToAmount)
	VALUES(@TimeStamp,@BaseCurrency,@BaseAmount,@ToCurrency,@ToAmount)

	select @@ROWCOUNT
END TRY  
BEGIN CATCH  
	THROW;
END CATCH
GO
/****** Object:  StoredProcedure [dbo].[usp_insert_conversion_rate]    Script Date: 2024/04/29 22:00:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<MP>
-- Create date: <2024-Apr-25>
-- Description:	<It will insert the Exchange Rates from api response.>
-- EXEC [dbo].[usp_insert_conversion_rate] @TimeStamp = '2024-01-01',@BaseCurrency = 'USD',@ExchangeRate = N'{"AED":3.6728,"AFN":71.999996}'
-- =============================================
CREATE PROC [dbo].[usp_insert_conversion_rate]
	@TimeStamp datetime,
	@BaseCurrency varchar(3),
	@ExchangeRate varchar(max)
AS

BEGIN TRY   

	INSERT INTO ExchangeRate
	(TimeStamp, BaseCurrency, ExchangeRate)
	VALUES (@TimeStamp,@BaseCurrency,@ExchangeRate)

	select @@ROWCOUNT
END TRY  
BEGIN CATCH  
	THROW;
END CATCH
GO
USE [master]
GO
ALTER DATABASE [CurrencyManager] SET  READ_WRITE 
GO

```
    
4 - Replace the connection string into launchSettings.json

```bash
  "ExchangeRateConnStr": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CurrencyManager;Integrated Security=True;Integrated Security=SSPI;",
```

5 - Rebuild the solution and then run it. It should open the swagger and you are ready to test.
## API Reference

#### Convert the amount

```http
  GET http://localhost:5048/Convert?basecode=CHR&targetcode=INR&amount=1
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `basecode` | `string` | **Required**. Base Currency Code, from which you want to exchange |
| `targetcode` | `string` | **Required**. Target Currency Code,to which you want to exchange |
| `amount` | `decimal` | **Required**. Amount that you want to convert |

#### Get the Conversion History

```http
  GET http://localhost:5048/Convert/GetConversionHistory?FromDate=2021-01-01
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `FromDate`      | `Date` | **Optional**. From which date you want to retrieve the conversion history.  |

#### Get the Conversion API History

```http
  GET http://localhost:5048/Convert/GetConversionAPIHistory?FromDate=2021-01-01
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `FromDate`      | `Date` | **Optional**. From which date you want to retrieve the conversion API history.  |


## Author

- Mehul Panchal

