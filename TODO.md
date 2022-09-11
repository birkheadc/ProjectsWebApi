## To do!

# Build the repository

- Do something to handle errors when attempting to create a table

# Logging
- Think about how to set up email with all of my projects, so that any project can send email to my personal email address if something critical happens.
- Figure out how to give the application permission to create directory and write logs to `/var/log/projectsWebApi/log.txt`
- Docker volumes always seem to be owned by root, I can't figure out how to give persmission to the user, so that logs can be made.

# Error handling
- A while back I attempted to build error-handling middleware but was unsuccessful. I should try again.

# Language Support
- The entire database needs to be rethought; shortDescription and longDescription need to have multiple values, for different languages. The model can be a dictionary like: `{['en', 'description'], ['jp', '説明']}`, but how should the database look?