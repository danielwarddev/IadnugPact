CREATE DATABASE pact_broker;
CREATE ROLE pact_broker WITH LOGIN PASSWORD 'password';
GRANT ALL PRIVILEGES ON DATABASE pact_broker TO pact_broker;