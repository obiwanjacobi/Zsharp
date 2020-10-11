# List

A List is an array that can change size during the execution of the code.

This ability comes with an extra cost.

It uses an Allocator to manage the memory the actual list is stored in as one sequence of data.

The List structure itself maintains a reference to that memory as well as some housekeeping variables.
