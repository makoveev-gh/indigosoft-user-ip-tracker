# User IP Tracker Service

This is a backend service for tracking users and their IP addresses. The service collects data from event sources about user connections with random IP addresses and stores them in a PostgreSQL database. It ensures efficient handling of high traffic, with features like searching for users by partial or full IP addresses, retrieving all IP addresses associated with a user, and finding the last connection time and IP address for a user.

## Features

- **Collects user IP address data**: The service listens for events and stores user connections along with their IP addresses.
- **Handles duplicates**: Multiple events with the same user and IP address are handled without issue.
- **Search by partial or full IP address**: You can search for users by partial IP address (e.g., "31.214"), and the service will return matching users.
- **Retrieves all IP addresses for a user**: The service can return a list of all previously connected IP addresses for a specific user.
- **Find the latest connection time and IP**: Retrieves the most recent IP address and connection time for a user.

## Key Components

### 1. **Event Processor**
The event processor collects data from queue, processes it, and stores it in the database. It ensures that when a user connects from a new IP address, the connection is recorded, and if the user reconnects from the same or a different IP, the timestamp of the latest connection is updated.

### 2. **Database Model**
The service uses PostgreSQL as the preferred database for storing data:
- **User IP Connections** are stored in a table with a unique combination of user ID and IP address.
- The table keeps track of the connection time (`Created`, `Updated` fields) and supports both IPv4 and IPv6.

### 3. **API and Queue Implementation**
- The service integrates with an **API** that handles requests for users' IP data. This allows external systems to query user IP addresses and connection times.
- Queue (EventQueue) processes incoming events, ensuring high performance even with large volumes of events. The queue user ConcurrentQueue, which is thread-safe and allow multiple threads to safely enqueue and dequeue items concurrently, providing efficient handling of events without the risk of race conditions. This design ensures that the system can scale effectively, even under high load, by processing a large number of events without causing delays or blocking the main thread.

### 4. **Performance and Scalability**
- The service is designed to handle peak loads efficiently

## Configuration

### 1. **Database Configuration**
The service can be configured with PostgreSQL database settings in the `app.json` file. It includes configurations for connecting to the database and managing the IP addresses stored.

### 2. **Event Processor Settings**
Settings for the event processor, such as:
- Time to wait when the event queue is empty.
- Time to wait between iterations of the event processing loop.
- Number of iterations before introducing a delay between processing events.

## API Endpoints

### 1. **Add or Update User IP Connection**
- **Endpoint**: `POST /api/v1/connections/add`
- **Description**: Adds a new user IP connection or updates an existing one.
- **Request Body**:
  ```json
  {
    "userId": 1234567,
    "ipAddress": "127.0.0.1"
  }

### 2. **Get All IPs Used by a Specific User**
- **Endpoint**: `GET /api/v1/connections/ips/{userId}`
- **Description**: userId (long) - The user ID to retrieve IP addresses for.
- **Response**: A list of all IP addresses associated with the specified user.
  ```json
  {
    ["127.0.0.1", "192.168.0.1"]
  }

### 3. **Get Last IP Connection of a User**
- **Endpoint**: `GET /api/v1/connections/lastConnection/{userId}`
- **Description**: userId (long) - The user ID to retrieve the last connection for.
- **Response**: The last IP address and connection timestamp for the user
  ```json
  {
    "ipAddress": "127.0.0.1",
    "connectedAt": "2025-03-24T12:00:00"
  }

### 4. **Search Users by IP Fragment**
- **Endpoint**: `GET /api/v1/connections/usersByIp/{ipFragment}`
- **Description**: ipFragment (string) - The IP address fragment to search for.
- **Response**: A list of user IDs matching the provided IP fragment.
  ```json
  {
    [1234567, 2345678]
  }

### 5. **Get All Users**
- **Endpoint**: `GET /api/v1/users/all`
- **Description**:  Returns a list of all users in the system.
- **Response**: A list of user IDs matching the provided IP fragment.
  ```json
  {
    [
        {"id": 1234567, "name": "User1"},
        {"id": 2345678, "name": "User2"}
    ]
  }

### 6. **Insert New User**
- **Endpoint**: `POST /api/v1/users/add`
- **Description**:  Inserts a new user into the database.
- **Request Body**: A list of user IDs matching the provided IP fragment.
  ```json
  {
    "id": 0,
    "name": "string"
  }

### Example Configuration:

```json
{
  "PostgreSql": {
    "Host": "localhost",
    "Port": 5432,
    "Username": "user",
    "Password": "password",
    "Database": "user_ip_tracker"
  },
  "EventProcessorSettings": {
    "EmptyQueueWaitTime": 1000,
    "IterationDelay": 500,
    "IterationCountBeforeWait": 100
  }
}
