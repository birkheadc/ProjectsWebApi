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
- Short and Long Descriptions -- short for a quick summary, like a sub-title; long for a full section on my 'My Projects' page
- Technologies -- a quick list of what tech was used to build it
- Site and Source -- links to the project's site / source code

## Repository

The repository (`IProjectRepository` / `ProjectRepository`) are built to make manual calls to a MySql database.

In order to do this, a `ConnectionString` must be established. This is done by reading certain environmental variables, then building the string at runtime. Those environmental variables must be established before compiling.

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

**In production, the following environmental variables must be established:**

- `MYSQL_SERVER`
- `MYSQL_PORT`
- `MYSQL_DATABASE`
- `MYSQL_USER`
- `MYSQL_PASSWORD`

These can be established via any method. If using Docker and docker compose, they can be passed from the docker-compose.yml, to the Dockerfile, and then to environmental variables.

## Database

This project makes manual calls to a MySql database. The database consists of three tables, which can be created via the following schema:

**Main Projects Table:**

```
CREATE TABLE IF NOT EXISTS `projects` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` tinytext,
  `short_description` text,
  `long_description` text,
  `site` text,
  `source` text,
  PRIMARY KEY (`id`)
  );
```

**Technologies Table:**

```
CREATE TABLE IF NOT EXISTS `technologies` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `name` (`name`)
);
```

**Project Technologies Join Table:**
```
CREATE TABLE IF NOT EXISTS `project_technologies` (
  `project_id` int NOT NULL,
  `technology_id` int NOT NULL,
  PRIMARY KEY (`project_id`,`technology_id`),
  KEY `technology_id` (`technology_id`),
  CONSTRAINT `project_technologies_ibfk_1` FOREIGN KEY (`project_id`) REFERENCES `projects` (`id`),
  CONSTRAINT `project_technologies_ibfk_2` FOREIGN KEY (`technology_id`) REFERENCES `technologies` (`id`)
);
```

Table schemas should be saved in `appsettings.json`, where they will be accessed at runtime. The repositories will then use this to create tables if they are not already present. A section in `appsettings` should be formatted like this:

```
"TableSchemasConfiguration": {
    "TableSchemas": {
      "{Projects}": [
        {
          "Name": "{projects}",
          "Schema": "{testschema}"
        },
        {
          "Name": "{technologies}",
          "Schema": "{testschema}"
        },
        {
          "Name": "{project_technologies}",
          "Schema": "{testschema}"
        }
      ]
    }
  }
```

Curly brackets denote arbitrary strings, otherwise the string must be *exactly as written* for ASP.NET to map the JSON to the object correctly. Even the arbitrary strings, however, are hard-coded in the related Repository classes, so care should be taken to make sure they match.
