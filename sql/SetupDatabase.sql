CREATE LOGIN environmentapp WITH PASSWORD='<password>';
CREATE DATABASE environmentdb;
USE environmentdb;
CREATE user environmentapp for login environmentapp;
GRANT control on DATABASE::environmentdb to environmentapp;