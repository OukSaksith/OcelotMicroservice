# OcelotMicroservice

A microservice API gateway powered by [Ocelot] for .NET, enabling efficient routing, authentication, and service aggregation.

## Getting Started

Follow these steps to start your OcelotMicroservice locally using Docker Compose:

### Prerequisites

- [Docker](https://www.docker.com/products/docker-desktop)
- [Docker Compose](https://docs.docker.com/compose/)
- [Git](https://git-scm.com/)

### 1. Clone the repository

```bash
git clone https://github.com/<your-username>/OcelotMicroservice.git
cd OcelotMicroservice
```

### 2. Configure Environment

Edit the `docker-compose.yml` as needed to match your environment and downstream services.

### 3. Start the microservice with Docker Compose

```bash
docker compose up --build
```
or for older Docker versions:
```bash
docker-compose up --build
```

This command will build and start all required services as defined in `docker-compose.yml`.

### 4. Access the API Gateway

Once all services are running, access the gateway at [http://localhost:5003](http://localhost:5003) (or the port specified in your `docker-compose.yml`).

### 5. Stop the services

To stop all running containers, press `Ctrl+C` in your terminal and then run:

```bash
docker compose down
```

---

For more information and advanced configuration, refer to the [Ocelot documentation](https://ocelot.readthedocs.io/).
