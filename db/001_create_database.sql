--DROP DATABASE "seek_and_destroy";

CREATE DATABASE "seek_and_destroy";

\c seek_and_destroy;

CREATE TABLE "users"
(
    "user_id" SERIAL PRIMARY KEY,
    "oauth_id" text UNIQUE,
    "email_address" text
);

CREATE TABLE "buildings"
(
    "user_id" integer references users (user_id) on delete cascade,
    "crystal_factories" integer
);

CREATE TABLE "resources"
(
    "user_id" integer  references users (user_id) on delete cascade,
    "crystals" integer
);