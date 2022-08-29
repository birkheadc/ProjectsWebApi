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

## Project Model

The `Project` model consists of all the information I need to display information about my personal software projects:

- ID -- A unique Guid for each project, to insure database rows don't clash, as well as give React a good Key when iterating over objects
- Name
- Short and Long Descriptions -- short for a quick summary, like a sub-title; long for a full section on my 'My Projects' page
- Technologies -- a quick list of what tech was used to build it
- Site and Source -- links to the project's site / source code
