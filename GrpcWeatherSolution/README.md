# GrpcWeather Microservice Example

## Overview

This project demonstrates a **simple gRPC-based microservice architecture** using **.NET 10**. It consists of two main components:

1. **GrpcWeatherServer** – A gRPC server providing weather information.  
2. **GrpcWeatherClient** – A gRPC client consuming the Weather microservice.

The system illustrates both **unary RPC** (single request-single response) and **server streaming RPC** (single request-multiple responses).

---

## Project Structure
```
GrpcWeatherSolution/
│
├─ GrpcWeatherServer/          # gRPC Weather microservice
│  ├─ Program.cs                # Server startup and configuration
│  ├─ WeatherService.cs         # Weather service implementation
│  └─ Protos/Weather.proto      # gRPC contract (messages + service definition)
│
├─ GrpcWeatherClient/           # Client consuming Weather service
│  └─ Program.cs                # Client implementation (unary + streaming)
```

---

## Getting Started

### Prerequisites

- .NET 10 SDK
- Visual Studio 2026 or VS Code
- Basic understanding of C# and console apps

---

### 1. Run the Server
```powershell
cd GrpcWeatherServer
dotnet run
```

### 2. Run the Client
```powershell
cd GrpcWeatherClient
dotnet run
```

Input a city name when prompted.

The client will perform:
- **Unary call** → fetches weather once for the requested city.
- **Server streaming call** → receives multiple weather updates from the server.

---

## How gRPC Works in This App

### Proto File (Weather.proto)

Defines the contract between client and server.

**Messages:**
- `WeatherRequest` → contains city
- `WeatherReply` → contains city, summary, temperatureC

**Services:**
- `GetWeather` → unary RPC
- `GetWeatherStream` → server streaming RPC

### Server Implementation

- `WeatherService.cs` overrides methods from `Weather.WeatherBase`.
- Generates random weather data.
- Returns either:
  - Single response (`GetWeather`) or
  - Multiple responses (`GetWeatherStream`) with delays.

### Client Implementation

- Connects to server via `GrpcChannel`.
- Uses generated `WeatherClient` to call RPC methods.
- Prints responses received from server.