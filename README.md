<!-- PROJECT LOGO -->
<br />
<div align="center">
    <h1 align="center">Toshokan</h3>

  <p align="center">
    About
An app that stores manga with automated updates, notifications, built in reader and more
  </p>
</div>


## Prerequisites

Prequisites to run Toshokan, basically .NET 6 runtime and dicjer

* docker
* dotnet6

## Installation

These are the steps to deploy Toshokan to docker
1. Install dotnet
    ```sh
    sudo apt-get update && sudo apt-get install -y dotnet6
    ```
2. Set https certificate
    ```sh
    dotnet dev-certs https --clean && dotnet dev-certs https -t
    ```
1. Clone the repo
   ```sh
   git clone https://github.com/emimontesdeoca/Toshokan.git
   ```
3. Build the containers
   ```sh
   docker-compose up -d
   ```

## Usage

When it's deployed, open the applicat

<!-- LICENSE -->
## License

Distributed under the Apache license. See `LICENSE` for more information.

