TonLib.NET
===========

Wrapper around `tonlibjson` library for accessing [Telegram Open Network](https://ton.org/) lite servers (nodes) directly via ADNL protocol.

**Does not** require TonAPI, TonCenter API, TonKeeper API or any other HTTP API. 

[![NuGet](https://img.shields.io/nuget/v/TonLib.Net.svg?color=blue)](https://www.nuget.org/packages/TonLib.Net/) ![NuGet downloads](https://img.shields.io/nuget/dt/TonLib.NET?color=blue) ![Framework](https://img.shields.io/badge/framework-net6.0-blue) ![Framework](https://img.shields.io/badge/framework-net7.0-blue) ![Framework](https://img.shields.io/badge/framework-net8.0-blue) ![GitHub License](https://img.shields.io/github/license/justdmitry/TonLib.NET?color=blue) 

⚠ For `net6.0` uses `System.Text.Json` package **v7.0.0** (from `net7.0`) - it makes [de]serialization much simpler (because of [Polymorphic serialization](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/polymorphism)). It only updates `System.Text.Encodings.Web` (v6.0 -> v7.0) as a transitive dependency, which I think is acceptable.


## Features

* Read account balance and transaction history [(sample)](/TonLibDotNet.Demo/Samples/AccountBalanceAndTransactions.cs);
* Operate with keys and mnemonics [(sample)](/TonLibDotNet.Demo/Samples/KeysAndMnemonics.cs);
* Send TON to different account [(sample)](/TonLibDotNet.Demo/Samples/SendTon.cs);
* Work with BOCs and Cells (read and parse using Slices, compose using CellBuilder) [(sample)](/TonLibDotNet.Demo/Samples/BocAndCells.cs), including HashmapE [(tests)](/TonLibDotNet.Tests/Cells/DictTests.cs);
* Read and parse smartcontract data, call get-methods [(sample)](/TonLibDotNet.Demo/Samples/ReadInfoFromSmartContracts.cs);
* Resolve domains [(sample)](/TonLibDotNet.Demo/Samples/ResolveDomains.cs)
* `TonRecipes` class (ready-to-use one-liners) to work with:
  * [TEP-81](https://github.com/ton-blockchain/TEPs/blob/master/text/0081-dns-standard.md) DNS contracts (parse all data, update entries) [(sample)](/TonLibDotNet.Demo/Samples/Recipes/RootDnsGetAllInfo.cs);
  * Telemint contracts (*.t.me usernames and +888 anonymous numbers) [(sample)](/TonLibDotNet.Demo/Samples/Recipes/Jetton.cs)
  * [TEP-74](https://github.com/ton-blockchain/TEPs/blob/master/text/0074-jettons-standard.md) Jettons (read info, transfer, burn) [(sample)](/TonLibDotNet.Demo/Samples/Recipes/TelemintGetAllInfo.cs)
  * [TEP-62](https://github.com/ton-blockchain/TEPs/blob/master/text/0062-nft-standard.md) NFTs (read info, transfer) [(sample)](/TonLibDotNet.Demo/Samples/Recipes/NFTs.cs)

And more:

* Easy-to-extend:
  * Describe and call new TonLib methods without waiting for new version [(sample)](/TonLibDotNet.Demo/Samples/LibraryExtensibility.cs);
  * Add your own Recipe methods to existing repice classes without recompiling library;
* Connects to random LiteServer or choosen by you;
* Reconnects to different LiteServer if previous one fails (but you need to handle exceptions and implement retry logic yourself, for example with [Polly](http://www.thepollyproject.org/));
* No 3rd-party packages [except native .NET assemblies](#3rd-party-libraries-and-dependencies);


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
```

Run [Demo project](/TonLibDotNet.Demo) for more samples. 


## Installing dependencies and running a demo

This library is a wrapper around `tonlibjson` library. You need to obtain complied copy of it (and its dependencies) yourself.

Go to https://github.com/ton-blockchain/ton/releases, open latest release, scroll to "Assets" and download `tonlibjson.*` for your OS. Make sure this file will be available for your running program (for example, add it to your project and set "Copy to Output Directory" to "Copy if newer").

The number of additional dependencies you need depends of what you already have on your machine. On my Win machine I also needed `libcrypto-1_1-x64.dll` from OpenSSL v1.1. You may use [Process Monitor](https://learn.microsoft.com/en-us/sysinternals/downloads/procmon) to find what it wants if it fails to run.


## 3rd-party libraries and dependencies

* Microsoft.Extensions.Logging.Abstractions v6.0.0 / v7.0.0 / v8.0.0
* Microsoft.Extensions.Options v6.0.0 / v7.0.0 / v8.0.0
* System.Text.Json v7.0.0 for `net6.0` and `net7.0`, v8.0.0 for `net8.0`
  * System.Text.Encodings.Web v7.0.0 for `net6.0` as transitive dependency


## Donate

`just_dmitry.ton`


## Useful links

* TON ADNL API Home: https://ton.org/docs/develop/dapps/apis/adnl
* TonLib TL Schema: https://github.com/ton-blockchain/ton/blob/master/tl/generate/scheme/tonlib_api.tl
* Block.tlb schema: https://github.com/ton-blockchain/ton/blob/master/crypto/block/block.tlb
* TL Language description: https://core.telegram.org/mtproto/TL