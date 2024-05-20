# Setting Up Stellar DataStore for StellarDsClient Application

Follow these steps to set up your Stellar DataStore account and configure the StellarDsClient application.

## Step 1: Create or Log in to a Stellar DataStore Account

1. **Create a new Stellar DataStore account** or **log in** to an existing one.

## Step 2: Add a New Application

1. Go to the **Applications** page.
2. Add a new application:
   - **Name**: `StellarDsClient`
   - **Redirect URL**: `[your domain name]/oauth/oauthcallback`
   - **Role**: `Admin`
3. Click **Create**.
4. Note the **redirect URL**, **client secret**, and **clientId** to build the `appsettings.Development.json` file in the application.

## Step 3: Configure Roles & Permissions

1. Go to the **Roles & Permissions** page.
2. Create a new role:
   - **Name**: `Readonly`
   - Disable all permissions except for:
     - Ignore the multitenant read behavior
     - See the table
     - See the records in the table
3. Adjust the **Admin** role:
   - Enable all permissions except for:
     - Update the metadata of the project
     - Delete the project
     - Ignore the multitenant read behavior
4. Adjust the **User** role:
   - Disable all permissions except for:
     - See the table
     - Create records in the table
     - See the records in the table
     - Delete records from the table
     - Update records in the table

## Step 4: Add an Access Token

1. Go to the **Access tokens** tab on the **Applications** page.
2. Add a new access token:
   - **Name**: `Readonly`
   - **Span**: `Unlimited` (the duration option will be ignored)
   - **Role**: `Readonly`
3. Create the token and note the **access token** to build the `appsettings.Development.json` file.

## Step 5: Create the appsettings.Development.json File

Create the `appsettings.Development.json` file in the StellarDsClient application and add the following JSON object:

```json
{
  "StellarDsClientSettings": {
    "ApiSettings": {
      "Project": "[The id of your project]",
      "ReadOnlyToken": "[The readonly access token]",
      "Tables": {
        "List": {
          "Name": "[A name unique to your project for the List table]"
        },
        "ToDo": {
          "Name": "[A name unique to your project for the ToDo table]"
        }
      }
    },
    "OAuthSettings": {
      "ClientId": "[The clientId]",
      "ClientSecret": "[The client secret]",
      "RedirectUri": "[the redirect URL]"
    }
  }
}

## Step 6: Start the solution

1. Start up the application and log in with your Stellar DataStore account.
2. The necessary tables will now be created automatically in your Stellar DataStore project.