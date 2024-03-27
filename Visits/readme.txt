This project works with SQLServer and SQLite with minimum changes.
It born working with SQLite and until this commit(git) 76f8ddcc1778a197003c9662395d08adf7d249b8 it still working with it.

Currently the EDMX was adjusted to work with SQLServer.
To change it to SQLite again you just need to delete the current EDMX and recreate it using the SQLite DB that can be generated using the PS script in the App_Data directory.

For more detail contact Ricardo Pool.