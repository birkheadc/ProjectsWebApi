## My 'Projects' Api

A web api for storing information about the projects I am working on, mainly used to programatically display them on my homepage.

# Focus
For this project, I am focusing on improving in the following areas:

## Documentation
I'm beginning to realize the importance of good documentation. It's not just so that an imaginary third party can understand my code; honestly I don't expect many people to ever visit or see this. It's important because it will help myself later.

There are so many small things I learn, especially when setting up a project, and not all of it is obvious just looking at the source code. I am tired of going through my own google search history to try and re-figure out the what and why of my not-even-that-old code.

Recently, I wrote a paragraph in a project's readme detailing how I set up dotnet secrets to store information needed to connect to the database in development. At the time, I was thinking *who am I kidding, no one is ever going to read this, and if they do, they won't need this explanation anyway.*

But six months later, when my old laptop crashed and I needed to clone that repository to my new computer, that single paragraph helped me immensely. I had heard people saying "documentation isn't just for the benefit of third parties, it's important for yourself as well." But it took until that moment to really *feel* the truth of it.

That's when I realized I needed to start practicing better documentation in general.

## Logging

I always try to work new best practices or frameworks into each project I start; something new to challenge myself, rather than simply writing what amounts to a slightly different version of the same project over and over.

In keeping with the spirit of better communication above, I've decided to build an actual logging system into this project. This isn't something I've read into how to do well yet, but I'm hoping that the simple nature of this API will lend itself to an easy first dive into logging.

# About the Code

This is a very standard CRUD repository, with a few simple API endpoints for manipulating a single table on a single database. I will use this API to update and display information regarding my personal projects on my website, a place to show off what I'm working on.

## Model

The `Project` model consists of all the information I need to display information about my personal software projects:

- ID -- A unique Guid for each project, to ensure database rows don't clash, as well as give React a good Key for iterating over objects
- Name
- ShortDescriptions -- a Dictionary of descriptions, with support for multiple languages. The key is the language, the value is the content of the description. Used for writing a quick blurb or subtitle for the project.
- LongDescriptions -- same as above, but containing a long form explanation of the project.
- Technologies -- a quick list of what tech was used to build it
- Site and Source -- links to the project's site / source code

## Repository

The repository (`IProjectRepository` / `ProjectRepository`) are built to make manual calls to a MySql database.

In order to do this, a `ConnectionString` must be established. This is done by reading certain environment variables, then building the string at runtime. Those environment variables must be established before compiling.

**In development, I suggest using dotnet user-secrets to do this:**
- `dotnet user-secrets init` to initialize user-secrets
- Take note of the following values:
    - **server**: the address of the server running MySql that we will use -- usually just `localhost` in development
    - **port**: MySql uses `3306` by default
    - **database**: the name of the database this application will use
    - **user**: the username of the user that the application will log in as -- recommend creating a user that only has privileges on the one database
    - **password**: the password for that user
- Create the following secrets:
    - `dotnet user-secrets set MySql:Server {server}`
    - `dotnet user-secrets set MySql:Port {port}`
    - `dotnet user-secrets set MySql:Database {database}`
    - `dotnet user-secrets set MySql:User {user}`
    - `dotnet user-secrets set MySql:Password {password}`

**In production, the following environment variables must be established:**

- `ASPNETCORE_MYSQL_SERVER`
- `ASPNETCORE_MYSQL_PORT`
- `ASPNETCORE_MYSQL_DATABASE`
- `ASPNETCORE_MYSQL_USER`
- `ASPNETCORE_MYSQL_PASSWORD`

These can be established via any method. If using Docker and docker compose, they can be passed from the docker-compose.yml, to the Dockerfile, and then to environment variables.

If not using docker compose, the arguments must be passed to the Docker build command via --build-arg. In the current Dockerfile, the Dockerfile arguments are the same as the environment variables, but without the `ASPNETCORE_` header. For example:

```
docker build -t projects:staging \
--build-arg MYSQL_SERVER=localhost \
--build-arg MYSQL_PORT=3306 \
--build-arg MYSQL_DATABASE=projects \
--build-arg MYSQL_USER=user \
--build-arg MYSQL_PASSWORD=passw0rd \
.
```

If running in a container, connecting to a MySql server can be slightly more difficult. If the application looks for a MySql server on localhost, it will search inside its container and likely not find anything. I recommend building another container for the MySql server, and connecting the two containers via a docker network. Then give the name of the MySql container as the MYSQL_SERVER.

## Database

This project makes manual calls to a MySql database. The database consists of five tables, which can be created via the following schema:

**Main Projects Table:**

The main table containing each project.

```
CREATE TABLE IF NOT EXISTS `projects` (
  `project_id` VARCHAR(36) NOT NULL,
  `project_name` tinytext,
  `site` text,
  `source` text,
  `is_favorite` tinyint(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (`project_id`)
  );
```

**Technologies Table:**

Contains a list of the technologies used in all projects. Used by a separate join table to relate them to their respective projects.

```
CREATE TABLE IF NOT EXISTS `technologies` (
  `technology_id` VARCHAR(36) NOT NULL,
  `technology_name` varchar(255) NOT NULL,
  PRIMARY KEY (`technology_id`),
  UNIQUE KEY `technology_name` (`technology_name`)
);
```



**Project Technologies Join Table:**

Joins projects and technologies in a many-to-many relationship.

```
CREATE TABLE IF NOT EXISTS `project_technologies` (
  `project_id` VARCHAR(36) NOT NULL,
  `technology_id` VARCHAR(36) NOT NULL,
  PRIMARY KEY (`project_id`,`technology_id`),
  KEY `technology_id` (`technology_id`),
  CONSTRAINT `project_technologies_ibfk_1` FOREIGN KEY (`project_id`) REFERENCES `projects` (`project_id`) ON DELETE CASCADE,
  CONSTRAINT `project_technologies_ibfk_2` FOREIGN KEY (`technology_id`) REFERENCES `technologies` (`technology_id`) ON DELETE CASCADE
);
```

**ShortDescriptions Table:**

A list of the short-descriptions related to each project in a one-to-many relationship.

```
CREATE TABLE IF NOT EXISTS `short_descriptions` (
  `short_description_language` varchar(255) NOT NULL,
  `short_description_content` text,
  `project_id` varchar(36) NOT NULL,
  PRIMARY KEY (`short_description_language`, `project_id`),
  KEY `fk_project_id` (`project_id`),
  CONSTRAINT `fk_short_description_project_id` FOREIGN KEY (`project_id`) REFERENCES `projects` (`project_id`) ON DELETE CASCADE
);
```


**LongDescriptions Table:**

A list of the long-descriptions related to each project in a one-to-many relationship.

```
CREATE TABLE IF NOT EXISTS `long_descriptions` (
  `long_description_language` varchar(255) NOT NULL,
  `long_description_content` text,
  `project_id` varchar(36) NOT NULL,
  PRIMARY KEY (`long_description_language`, `project_id`),
  KEY `fk_project_id` (`project_id`),
  CONSTRAINT `fk_long_description_project_id` FOREIGN KEY (`project_id`) REFERENCES `projects` (`project_id`) ON DELETE CASCADE
);
```
### Adding languages
To add additional languages to Short- and LongDescriptions, just add a new row with the same project_id, including the language as a short string in `{short/long}_description_language`.

### Removing languages
To remove a language: update the language while specifically setting the content of the language to a blank string. (Failing to include the language at all will cause the program to completely ignore that KeyValue pair, meaning nothing will be changed.)

For example, if you wish to remove "german" from the list of short descriptions for a project, be sure to include `"german": ""` under `shortDescriptions` in the JSON, like so:
```
"shortDescriptions": {
  "german": "",
  "english": "my new english description",
  ...
}
```

Remember, omitting "german" entirely will **not** remove it from the project. It must be explicitly included and marked as empty, or the repository will ignore it.

### Changing the schemas
Table schemas should be saved in `appsettings.json`, where they will be accessed at runtime. The repositories will then use this to create tables if they are not already present. A section in `appsettings` should be formatted like this:

```
"TableSchemasConfiguration": {
    "TableSchemas": {
      "{Projects}": [
        {
          "Name": "{projects}",
          "Schema": "{schema}"
        },
        {
          "Name": "{technologies}",
          "Schema": "{schema}"
        },
        {
          "Name": "{project_technologies}",
          "Schema": "{schema}"
        },
        {
          "Name": {short_descriptions}",
          "Schema": "{schema}"
        },
        {
          "Name": {long_descriptions}",
          "Schema": "{schema}"
        }
      ]
    }
  }
```

Curly brackets denote arbitrary strings, otherwise the string must be *exactly as written* for ASP.NET to map the JSON to the object correctly. Even the arbitrary strings, however, are hard-coded in the related Repository classes, so care should be taken to make sure they match.

This application has no way of changing any tables that are already present. Therefore, when updating or making changes to the schemas, the relevant tables should be manually dropped from the mysql database. Failure to do so will likely result in the app crashing, because the tables will be in an unexpected format.

(It would be nice to build a method for having the app reformat its tables on its own, but I felt this was unsafe, as there is the likelyhood of data being lost this way.)

# Security

This app simply checks a password supplied by the user against an internal password held in an environment variable. This is not very secure but this app is not meant to be very secure. It is just a simple single step to prevent random people from modifying the database.

In development:

`dotnet user-secrets set Password {password}`

In production, use the same method as when setting up the database variables:
`--build-arg PASSWORD={password}`

Use a random string as a password. Store that string in the database of the app that you use to update this app. Use a correctly hashed and secured password that you physically type into that app, to retrieve this app's password.

# Logging

This app uses Serilog for logging. The following NuGet packages are used:
- `Serilog.AspNetCore 6.2.3`
- `Serilog.Settings.Configuration 3.3.0`
- `Serilog.Enrichers.Environment 2.2.0`
- `Serilog.Enrichers.Thread 3.1.0`
- `Serilog.Enrichers.Process 2.0.2`

Logging configuration is set to be read from the `"Serilog"` section in `appsettings.json`.

Currently, logging is sent to the console, as well as written in JSON format to a daily log_{date}.json file.

## Logging & Docker Volumes
When building the app with `docker compose`, in order to write these log files to a volume so that they can be accessed outside of the container, I have been forced to run the app as root inside its container.

This seems dangerous, but so far I have not found a better solution. It appears that no matter what I do, the file inside the container that is designated as a volume is always owned by root, and cannot be written to by any other user.

I was confused at first, because my MySql containers have never had this problem. But upon further inspection, that was because my MySql containers had been running as root the entire time.

I suspect it may be possible to grant certain permissions on the HOST file-system to specific user Ids, then making certain that the users in the containers have the same Ids. But this coupling of host to container leaves a sour taste in my mouth; it seems to run opposed to the very point of containerization.

I'm sure there's a good solution somewhere, but at the moment I have simply resigned myself to letting the apps run as root in their containers.