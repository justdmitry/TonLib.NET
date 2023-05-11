TonLib.NET
===========

Wrapper around `tonlibjson` library for accessing [Telegram Open Network](https://ton.org/) lite servers (nodes) via ADNL protocol.

[![NuGet](https://img.shields.io/nuget/v/TonLib.Net.svg?color=blue)](https://www.nuget.org/packages/TonLib.Net/) ![NuGet downloads](https://img.shields.io/nuget/dt/TonLib.NET?color=blue) ![Framework](https://img.shields.io/badge/framework-net6.0-blue) ![GitHub License](https://img.shields.io/github/license/justdmitry/TonLib.NET?color=blue) 

⚠ Uses `System.Text.Json` package **v7.0.1** (from `net7.0`) - it makes [de]serialization much simpler (because of [Polymorphic serialization](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/polymorphism)). It only updates `System.Text.Encodings.Web` (v6.0 -> v7.0) as a transitive dependency, which I think is acceptable.

## Features

* Read account balance and transaction history;
* Operate with keys and mnemonics;
* Send TON to different account;
* Send arbitrary message to account;
* Read and parse smartcontract data, call get-methods;
* Read DNS data (resolve to ADNL);
* Work with BOCs and Cells (read and parse using Slices, compose using CellBuilder);

And more:

* Easy-to-extend: describe and call new TonLib 
* Connects to random LiteServer or choosen by you;
* Reconnects to different LiteServer if previous one fails (but you need to handle exceptions and implement retry logic yourself, for example with [Polly](http://www.thepollyproject.org/));
* No 3rd-party packages;

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


## Installing dependencies and running a demo

This library is a wrapper around `tonlibjson` library. You need to obtain complied copy of it (and its dependencies) yourself.

Go to https://github.com/ton-blockchain/ton/releases, open latest release, scroll to "Assets" and download `tonlibjson.*` for your OS. Make sure this file will be available for your running program (for example, add it to your project and set "Copy to Output Directory" to "Copy if newer").

The number of additional dependencies you need depends of what you already have on your machine.

When something is missed, Demo app will fail with "Unable to load DLL 'tonlibjson' or one of its dependencies" exception. You may use [Process Monitor](https://learn.microsoft.com/en-us/sysinternals/downloads/procmon) to find what file it wants.

On my Win machine I needed `libcrypto-1_1-x64.dll` from OpenSSL.

## Donate

`EQDP_JFi4IucdPmEHJBSjEoSHlbC57G_8HLJAgqe05a-sGZ8`

## Useful links

* TON ADNL API Home: https://ton.org/docs/develop/dapps/apis/adnl
* TonLib TL Schema: https://github.com/ton-blockchain/ton/blob/master/tl/generate/scheme/tonlib_api.tl
* TL Language description: https://core.telegram.org/mtproto/TL