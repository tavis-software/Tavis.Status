Status Documents for HTTP Resources
====================================

Abstract
---------
This document proposes a generic "status document" to provide user-agents with a consistent method to monitor the current status of web based resources. 

Introduction
------------
There are numerous client scenarios, like dashboards, status bars, progress windows where it is desirable to indicate the status of some remote resource.  There are many different types of resource that could be monitored : servers, running reports, builds, external hardware.  This media type provides a simple format that can be used by generic user-agent code to monitor an arbitrary resource.  

Primarily this will be for the purpose of displaying information to a human.  However, the resource defines a simple state machine that is sufficiently generic but contains enough semantics to be actionable by a user-agent to automatically respond to certain state changes.


Status Documents
----------------

A status document might look like:

    <status state="busy" progress="2/75" message="This is a message"/>

It consists of a root node named `status` and has the properties: `state`, `progress` and `message`.  

Only the `state` property is required, and should be one of the following values : {`waiting`, `busy`, `warning`, `error`, `ok`, `cancelled`}

The `progress` property is a fraction that can be optionally used to indicate to a user how much longer we expect the resource to remain in a particular state.

The `message` property is a human readable string intended to qualify the resource state.

Additional information can be provided by embedding links within the status node.

    <status state="ok" message="Finshed generating report">
        <link rel="related" href="http://example.org/report/99/output"/>
    </status>

the JSON equivalent of this would be

    { "status" : {
    		"state" : "ok",
    		"message" : "Finished generating report",
    		"links" : {
    			"related" : { "href" : "http://example.org/report/99/output"}
    		}
    	}
    }

The syntax of the links object in the JSON format follows the convention of HAL except that there is no leading underscore for the links object.  The underscore is unncessary as this media type does encourage embedding arbitrary additional content, therefore no naming conflicts should occur.  

The specification does not prohibit extensions to the content, however it defines no rules for extensions beyond the constraints of the underlying JSON/XML format. 


Media Type Identifiers
-----------------------
The intent is to submit this specifcation as a proposal to IANA for registration using the following identifiers:

* application/status+xml
* application/status+json

