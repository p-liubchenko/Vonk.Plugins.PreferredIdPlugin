# Vonk.Plugins.PreferredIdPlugin [![.NET](https://github.com/p-liubchenko/Vonk.Plugins.PreferredIdPlugin/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/p-liubchenko/Vonk.Plugins.PreferredIdPlugin/actions/workflows/dotnet.yml)

## Server changes
1. Added plugin assembly name to /administration path
2. Added operation name to SupportedInteractions > TypeLevelInteractions

## Tested
1. `curl  -X GET \
  'http://localhost:4080/administration/NamingSystem/$preferred-id?id=http%3A%2F%2Fhl7.org%2Ffhir%2Fsid%2Fus-ssn&type=oid'`
2. `curl  -X GET \
  'http://localhost:4080/administration/NamingSystem/$preferred-id?id=http%3A%2F%2Fhl7.org%2Ffhir%2Fadministrative-gender&type=uri'`
3. `curl  -X POST \
  'http://localhost:4080/administration/NamingSystem/$preferred-id' \
  --header 'Accept: application/fhir+json; fhirVersion=4.0; charset=utf-8' \
  --header 'Content-Type: application/fhir+json; fhirVersion=4.0; charset=utf-8' \
  --data-raw '{
  "resourceType" : "Parameters",
  "parameter" : [ {
     "name" : "id",
     "valueString" : "http://hl7.org/fhir/sid/us-ssn"
   }, {
     "name" : "type",
     "valueCode" : "oid"
   }
  ]
}'`
