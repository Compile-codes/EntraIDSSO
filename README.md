# 🔐 EntraID SSO Utility for MLVizz

Welcome to the **EntraID SSO Authentication Utility**.  
This is a lightweight .NET 9.0-based application designed to demonstrate and implement Single Sign-On (SSO) using **Microsoft Entra ID (Azure Active Directory)**.

## 📌 Purpose

This tool allows you to:

- Redirect a user to Microsoft Entra ID login page
- Capture the authorization code after successful login
- Exchange the code for an access token
- Validate the token and simulate session handling

---

## 👨‍💻 Developer

**Joel Anil Antony** – Data Engineer Intern - Simply AI

---

## 📦 Tech Stack

- [.NET 9.0](https://dotnet.microsoft.com/)
- ASP.NET Core (Kestrel Server)
- Microsoft Identity Client (MSAL.NET)
- JSON Web Token (JWT) Parsing

---

## 🚀 How It Works (Non-Tech Summary)

1. You start the app by running it on your computer.
2. The app shows you a special Microsoft login link.
3. You open that link in your browser and sign in.
4. Microsoft sends a code back to your app.
5. The app uses that code to get an access token (like a secure pass).
6. It checks the token to make sure it's valid and shows confirmation.

---

## 🛠️ Installation Instructions

### ✅ Prerequisites

Make sure you have the following installed:

- [.NET 9.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- Git (optional, for cloning)
- A terminal (Command Prompt, PowerShell, or Terminal on Mac)

---

### 📥 Step-by-Step Setup

#### 1. Clone or Download the Repository

```bash
git clone https://github.com/Compile-codes/EntraIDSSO.git
cd EntraIDSSO
```

#### 2. Set Environment Variable (🔑 Client Secret)

Create an environment variable with your Azure app's client secret.

**For Windows:**

```powershell
$env:AZURE_CLIENT_SECRET="your-client-secret-here"
```

**For macOS/Linux:**

```bash
export AZURE_CLIENT_SECRET="your-client-secret-here"
```

> 🔒 **Note**: Never share this secret. Treat it like a password.

#### 3. Run the App

```bash
dotnet run
```

#### 4. Follow the Console Instructions

- Copy the Microsoft login link printed on the console.
- Open it in a browser and log in.
- Wait for the redirect to `http://localhost:5000/callback` – the app will handle the rest.

---

## 🔍 Example Console Output

```text
Starting EntraID SSO Application...
Please visit the following URL to log in: https://login.microsoftonline.com/...
After logging in, the browser will redirect...
Kestrel server is running. Awaiting callback...
```

After login:

```text
Received authorization code: xxxxxx...
Access token is valid!
Access token acquired successfully! Redirecting user to the main application...
```

---

## 🧪 Testing

This utility is designed to be run locally and simulate real-world SSO login flows. It's safe for demonstration and internal use.

---

## ❓ FAQ

**Q: Do I need to deploy this app?**  
A: No. It runs locally for demo or internal tool purposes.

**Q: Will this work with any Azure AD tenant?**  
A: Yes, as long as you register the app in Azure AD and configure the proper redirect URI and permissions.

**Q: Can I use this in production?**  
A: This is a **demo-grade utility**. For production, include full token validation using Microsoft’s JWKS endpoint and secure HTTPS transport.

---

## 📬 Contact

For suggestions, bugs, or collaboration, reach out to:

📧 [jantony@simplyai.com.au]

---

## 📝 License

This project is open for internal use and learning. Feel free to modify it as needed, but respect the Microsoft license terms when using SDKs and packages.
