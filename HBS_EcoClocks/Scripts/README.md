**DATABASE PROJECT WORKFLOW**

Overview

The Database Project operates by reading a set of SQL scripts from a
designated folder. These scripts are carefully named to ensure the
correct execution order. The project keeps track of executed scripts,
running only those that haven\'t been executed before.

Branching Strategy

**Step 1: Initial Branch Setup**

Developers initiate the process by pulling the \'dev\' branch from the
repository.

**Step 2: Task-Specific Branch Creation**

After that, they will create a new branch with a name based on the
assigned EP (Epic or Story name) for their specific task.

**Step 3: Integration with \'dev\'**

Each developer will then perform a merge into the \'dev\' branch.

**Step 4: Sequential Merges**

Following these individual merges, there is a series of subsequent
merges:

-   \'dev\' branch to \'qa\' branch

-   \'qa\' branch to \'staging/uat\' branch

-   \'staging/uat\' branch to \'master\' branch for the production
    environment.

**Conclusion: Ensuring Backup**

This systematic approach ensures that the \'dev\' branch serves as a
secure backup branch throughout the development process.

**Key Points**

When creating a .sql file in a Db project script folder, consider the
following key points:

-   Use a descriptive filename that clearly indicates the script\'s
    purpose (e.g., create_table_users.sql).

-   Start the filename with a number for proper execution order (e.g.,
    001_create_table_users.sql).

-   Utilize comments to explain the script\'s functionality and purpose.

-   Thoroughly test scripts to prevent bugs from entering the database.

-   Test changes before merging into the development branch to avoid
    introducing bugs into the production environment.

**Database Script Remediation Process**

When addressing a bug in a script, updating it is essential. However,
after the update, the script may not run automatically if previously
executed. To address this:

-   Manually execute the updated script in the development environment.

-   Avoid creating a new file solely for remediation purposes, as the
    system will execute the bug-containing file first during the merge
    to the QA branch and subsequently execute the resolution file.
