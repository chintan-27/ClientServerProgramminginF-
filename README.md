# Client-Server Programming in F#

![F# Logo](https://upload.wikimedia.org/wikipedia/commons/6/66/F_Sharp_logo.svg)

## Overview

This repository contains code for Client Server Programming in F#. This is a project for the course Distributed Operating Systems Principle COP5616 at University of Florida.

## Contents

- **Client-Side**
  - Explore F# for client-side programming, including:
    - **Web Client**: Creating web clients using F# for consuming REST APIs or WebSocket services.
    - **Desktop Applications**: Building cross-platform desktop applications with F# using libraries like Avalonia or Xamarin.Forms.
    - **Mobile Apps**: Developing mobile applications using F# with Xamarin or Fable.
  
- **Server-Side**
  - Dive into server-side programming in F#, including:
    - **HTTP Servers**: Building HTTP servers and RESTful APIs using libraries like Suave or Giraffe.
    - **WebSocket Servers**: Creating WebSocket servers for real-time communication.
    - **Database Integration**: Connecting to databases using F# and interacting with them.
  
- **Examples**
  - A collection of well-commented code examples and projects to help you learn and understand F# for client-server programming.

## Getting Started

To get started with this repository, follow these steps:

1. Clone the repository to your local machine:

   ```sh
   git clone https://github.com/YourUsername/ClientServerProgramminginFSharp.git
2. Navigate to the directory
    ```sh
    cd ..<directory name>
3. Make sure you have .Net in your OS
    ```sh
    dotnet --version
4. Build the project
     ```sh
    dotnet build
5. Run as a server
     ```sh
    dotnet run server
6. Run as a client. You can have more than one clients. You can communicate with the server simultaneously
     ```sh
    dotnet run client

## Details

In this project, we have developed a concurrent client-server implementation for socket communication using F#. The project includes both server and client programs written in F# to facilitate communication via TCP/IP sockets. The server handles concurrent client requests, responding to arithmetic commands like addition, subtraction, and multiplication, followed by 2 to 4 input numbers. It performs error handling, returning appropriate error codes for incorrect commands. Graceful termination is ensured, allowing clients to exit individually with a "bye" command or terminate all connections with a "terminate" command. The program follows the specified input-output format and port number constraints. This assignment demonstrates effective socket programming and exception handling in F#.