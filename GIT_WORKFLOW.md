# Git Workflow

## *Clone git project to local computer*

To get started you should clone the project repository to a local folder on your computer. 

- [Cloning a git repository in Visual Studio Code](https://code.visualstudio.com/docs/sourcecontrol/quickstart)
- [Cloning a git repository in Visual Studio Community](https://learn.microsoft.com/en-us/visualstudio/version-control/git-clone-repository?view=visualstudio)

## Create new branch

When starting a new task you should always create a new local branch. Usually this means branching from the 'Main' or 'Master' branch.

- [Creating a new branch in VS Code](https://code.visualstudio.com/docs/sourcecontrol/branches-worktrees)
- [Creating a new branch in Visual Studio Community](https://learn.microsoft.com/en-us/visualstudio/version-control/git-create-branch?view=visualstudio)


## Naming conventions

When working with Git, especially in a team, you should follow some standard naming conventions:

**feature/*new-branch-name***: New/changed functionality

**bugfix/*new-branch-name***: Fix existing functionality

**docs/*new-branch-name***: Adding documentation, e.g. Readme files

- [Article on Git naming convention](https://medium.com/@abhay.pixolo/naming-conventions-for-git-branches-a-cheatsheet-8549feca2534)

## Stage, Commit and Pull Request
After making changes you should stage them, add a short commit message and push the changes to remote. This uploads your branch to Github and ensures your changes are backed up. When the work/task associated with the branch is completed you should create a pull request and add your team members as reviewers.

- [Workflow VS Code](https://code.visualstudio.com/docs/sourcecontrol/repos-remotes)
- [Workflow Visual Studio Community](https://learn.microsoft.com/en-us/visualstudio/version-control/git-make-commit?view=visualstudio)