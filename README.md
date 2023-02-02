TonLib.NET
===========

Wrapper around `libtonlibjson` library for accessing [Telegram Open Network](https://ton.org/) lite servers (nodes) via ADNL protocol.

**Important!** You need to obtain compiled `tonlibjson.dll` (or appropriate for your OS) and its dependencies (e.g. `libcrypto-1_1-x64.dll`) yourself. See below for details.

[![NuGet](https://img.shields.io/nuget/v/TonLib.Net.svg?maxAge=86400&style=flat)](https://www.nuget.org/packages/TonLib.Net/) 


⚠ Uses `System.Text.Json` package **v7.0.1** (from `net7.0`) - it makes [de]serialization much simpler (because of [Polymorphic serialization](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/polymorphism)). It only updates `System.text.Encodings.Web` (v6.0 -> v7.0) as a transitive dependency, which I think is acceptable.

## Usage

Register in `Startup` (for console projects - create instance manually or see demo project for hosted sample):

```csharp
services.AddSingleton<ITonClient, TonClient>();
```

And use:

```csharp
// Obtain client from DI
var tonClient = app.Services.GetRequiredService<ITonClient>();

// You need to init it before first use.
// During this, TON network config file is loaded from internet.
// Subsequent calls to `InitIfNeeded` will be ignored, 
//   so no need for you to have additional variable 
//   to remember that you already called it.
await tonClient.InitIfNeeded();

// Use 'Execute' to send requests.
var lsi = await tonClient.Execute(new LiteServerGetInfo());
logger.LogInformation("Server time: {Now}", lsi.Now);

var mi = await tonClient.Execute(new GetMasterchainInfo());
logger.LogInformation("Last block: shard = {Shard}, seqno = {Seqno}", mi.Last.Shard, mi.Last.Seqno);
```

And the result is:

![Sample](https://raw.githubusercontent.com/justdmitry/TonLib.NET/master/README_sample.png)

⚠ `Execute` requests are not truly asynchronous now - calls to tonlib are synchronous, and only waiting between them is a reason for 'async'. Help is wanted to make `Execute` really asynchronous.


## Installing dependencies and running a demo

This library is a wrapper around `tonlibjson` library. You need to obtain complied copy of it (and its dependencies) yourself.

### 1. Obtaining `tonlibjson` library

#### Option 1: Build it yourself

1. Checkout https://github.com/ton-blockchain/ton
2. Follow manual at https://ton.org/docs/develop/howto/compile and build `tonlib` target.

#### Option 2: Get compiled one from GitHub Actions

1. Open https://github.com/ton-blockchain/ton/actions
2. Choose your OS (one that matches better) in left "Actions" (Workflows) menu
3. Look for run with tag named like `v2023.01` (or newer) and open that build/run.
4. Scroll down to "Artifacts" section, you'll find something like `ton-win-binaries` file about 70MB in size. Download it.
5. Open downloaded archive and extract `tonlibjson.dll` (or similair for your OS).
6. Make sure this file will be available for your running program (for example, add it to your project and set "Copy to Output Directory" to "Copy if newer").

### 2. Obtaining dependencies

The number of additional dependencies you need depends of what you already have on your machine.

When something is missed, Demo app will fail with "Unable to load DLL 'tonlibjson' or one of its dependencies" exception. You may use [Process Monitor](https://learn.microsoft.com/en-us/sysinternals/downloads/procmon) to find what file it wants.

On my Win machine I needed `libcrypto-1_1-x64.dll` from OpenSSL.


## Useful links

* TON ADNL API Home: https://ton.org/docs/develop/dapps/apis/adnl
* TonLib TL Schema: https://github.com/ton-blockchain/ton/blob/master/tl/generate/scheme/tonlib_api.tl
* TL Language description: https://core.telegram.org/mtproto/TL