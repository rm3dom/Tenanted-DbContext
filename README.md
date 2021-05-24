# Multi tenancy using multiple DbContext's

Example app showing how to do multi tenancy in .Net Core using multiple databases. 
When you have limited clients and a legacy system this is a workable approach.
Ultimately you would want to move to a tenanted data store per micro service.

## build_and_run.sh
* Compile and create a container with the API
* Sets up K8 cluster using `kind`
  * Deploy the API with 2 replicas
  * Deploys multiple MySQL DB's with their own init.sql script 
    * Each tenant db will have a hello table with sayings of the form "Hello Tenant{X}"
* Runs a curl GET against each tenant.
* Destroys the cluster.

This was tested on a laptop with 12 cores, 32GB memory and 20 tenant database's. 
Your pods may get killed of when you specify to many.

## Requirements
* kubectl
* kind
* docker

## Video

[![Video](https://drive.google.com/file/d/1xdcdU6G0XbltYeaUhu6PryFvfh2ubSVR/view?usp=sharing)](https://drive.google.com/file/d/1owEjXNI4fz07Eyi4YW5kqfvMI8HBwKXr/view?usp=sharing)


