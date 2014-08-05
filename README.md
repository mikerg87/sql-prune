## SQL Pruning Utility

A simple utility written in .NET to to prune MS-SQL backup files from a given folder (or an Amazon S3 bucket).

The program lists all Microsoft SQL Server `.bak` files in a given folder or Amazon S3 bucket and determines which one to keep or prune.

#### Disclaimer

**WARNING**: This program is designed to delete files. The authors of this program do not accept liability for any errors or data-loss which could arise as a result of it.

#### Pruning Rules

1. Keep daily backups for two weeks
2. Keep One Sunday per week for eight weeks
3. Keep 1st and 3rd Sunday of each month for 52 weeks
4. Keep 1st Sunday of each year after that
6. When more than one backup per day, keep the most recent.
5. Prune anything else

**Notes:** 

The following pruning rules apply:

- Per **database**
- Only support the _Julian_ calendar 
- Apply from the _date of the most recent backup_ for a given database

Example of expected file names (as automatically generated by backup plans created in Microsoft SQL Management Studio):

    dbname1_backup_2014_06_20_010002_0897411.bak
    dbname2_backup_2014_06_20_010002_0957417.bak
    ...

The utility relies on the date *in the file name*, **not** the file system's creation date.

#### Usage:

    sqlprune.exe [path] [-delete] [-no-confirm] [-aws-profile]

 * __path__ is the path to a local folder or an S3 bucket containting .bak files (e.g. `c:\sql-backups` or `s3://bucket-name/backups`)
 * __-delete__ is a flag you must add otherwise files will not be deleted
 * __-no-confim__ is flag you can if you don't want to confirm before any file is deleted
 * __-aws-profile__ is optional and defaults to the value to the `AWSProfileName` app setting (see S3 Credentials)

Examples:

Simply list which files would be pruned in a folder without deleting anthing (dry run):

    sqlprune E:\Backups

Confirm before deleting prunable backups in `E:\Backups`, including sub directories:

    sqlprune E:\Backups -delete

Confirm before deleting prunable backups for database names starting with `test` in `s3://bucket-name`:

    sqlprune s3://bucket-name/test -delete

#### Download & Install:

1. Find the [latest release](https://github.com/comsechq/sql-prune/releases).
2. Extract the zip in a folder.
3. Run the command from the command line prompt.

#### S3 Credentials

You can ignore this completely if you just want to prune files from a local folder.

sqlprune.exe loads the amazon credentials it needs to connect to S3 a
[configuration file](http://docs.aws.amazon.com/cli/latest/userguide/cli-chap-getting-started.html) located in `~/.aws/config`, using the `default` profile.

Example:

    [default]
    aws_access_key_id = XXXXXXXXXXXXXXXXXXXX
    aws_secret_access_key = YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY

You can have more than one profile in your configuration file.

To override the default profile name, either modify the `AWSProfileName` setting  in `sqlprune.exe.config` or alternatively add an `-aws-profile` parameter when you execute `sqlprune.exe` from the command line.

You can also modify the `AWSProfilesLocation` setting in `sqlprune.exe.config` to load a different file (e.g. "c:\aws\config").

#### TODO:

- Cutomisable pruning rules: 
    - Load the rules from an XML configuration file
    - Generate an XSD that describes the XML syntax for pruning rules 
    - The pruning rules in the configuration file are applied one after the other

#### Unit Testing

Veryfying which date should be kept or pruned is a tedious task.

To make it easier, we have modified an [SVG visualiser](http://bl.ocks.org/mbostock/4063318) 
to render the output of the unit tests into a familiar calendar view.

After you run the unit tests in `PruneServiceTest`, open `Calendar.html` in a modern web browser.

_If your web browser doesn't have access to you local file system (e.g. Chrome) it will refuse to load the .json file._

Example:

![alt tag](https://raw.githubusercontent.com/comsechq/sql-prune/master/unit-test-output-example.png)