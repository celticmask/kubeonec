#!/bin/bash
ras cluster --daemon
ragent -d $ONEC_DATA $@
