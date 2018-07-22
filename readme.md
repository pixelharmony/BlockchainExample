# Blockchain Example

A quick example of setting up a basic P2P blockchain in dotnet core. This is purely a basic example and is by no means complete. Potential issues/improvements will be listed below

## Getting Started

To run this project you will need the dotnet core 2.1 SDK installed. If you haven't use this SDK before, see [here](https://www.microsoft.com/net/learn/get-started-with-dotnet-tutorial) for instructions on how to get started.

- Build the application to a DLL using `dotnet publish` in the root directory.
- Start as many instances of the application as you like by running `dotnet BlockchainExample.Server.dll` in the publish output directory (using seperate console windows)

## Issues

- ~~You must run 3 instances of the app. The ports/instances are hardcoded and they all need to be there when you mine a new block as it will try to update the entire network~~ - Added service discovery
- If you close the app too quickly after stopping the server, it will sometimes give you an "ObjectAlreadyDisposed" Exception

## Possible Improvements

- ~~Add service discovery so that new nodes can be detected as they are added. This would remove the need for hardcoded nodes~~ - Done
- Non-local IP addresses. Currently it's hardcoded to 127.0.0.1 but it would be nice to be able to access it externally so that each instance could be run in a container or on another machine
- General code clean up
- Tests (I didn't put any in since it was just a quick example but if I expand it any further I will put tests in)
- Currently blocks are created without any data which makes them a bit pointless. This needs to be expanded so that they contain transaction data

## Authors

* **Scott Bamforth** - *Initial work* - [Pixel Harmony](https://github.com/pixelharmony)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
