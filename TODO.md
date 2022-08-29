## To do!

# Build the repository

- Figure out the connection string, how to store / retrieve that string during development / production.
- Build the RepositoryBase class, figure out:
    - How to build the table if it is missing
    - How to check that the table is of the correct format if it exists
    - What to do if the table is present but not compatible with the Model. If the table is empty, maybe drop it and rebuild, but what if it has data?

# Logging
- Learn how to do it.
- Do it.
- Think about how to set up email with all of my projects, so that any project can send email to my personal email address if something critical happens.