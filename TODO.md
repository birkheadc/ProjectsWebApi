## To do!

# Build the repository

- Build the RepositoryBase class, figure out:
    - How to build the table if it is missing
    - How to check that the table is of the correct format if it exists
    - What to do if the table is present but not compatible with the Model. If the table is empty, maybe drop it and rebuild, but what if it has data?
- Do something to handle errors when attempting to create a table
- Add actual schema strings to appsettings.json, update readme as well, make sure tables are generated correctly

# Logging
- Learn how to do it.
- Do it.
- Think about how to set up email with all of my projects, so that any project can send email to my personal email address if something critical happens.

# Error handling
- A while back I attempted to build error-handling middleware but was unsuccessful. I should try again.

# Authorization

- Only my authorized apps should have access to the API, so I need a method for authorizing them.