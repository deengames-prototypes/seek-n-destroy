CREATE DATABASE "seekAndDestroy";

\c seekAndDestroy;

CREATE TABLE "Buildings"
(
    "userId" integer,
    "crystalFactories" integer
);

CREATE TABLE "Resources"
(
    "userId" integer,
    "crystals" integer
);

CREATE TABLE "Users"
(
    "userId" integer,
    "emailAddress" text
);