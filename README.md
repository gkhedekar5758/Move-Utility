# Move-Utility

### Problem statement: -
When developer is working on the code changes, (s)he has to manage many version code check-in (usually in MNCs like mine has many versions of software are running parrallaly in development). For example, a project (or product) might have many versions open (for e.g. in the product that I am working on has many versions
parallelly in a year). When user makes a code change and check-in in one version that time (s)he has to make check in other versions as well to keep the codebase same. 
 
### Here are the problem or manual task that (s)he has to follow
- Developer needs to map and download the code files of new branch in his/her local machine
- (s)he needs to make changes in code by using IDE
- (s)he needs to check-in the code and associate it with a bug/PBI.
- The information of check in needs to send out to project managers, scrum masters and other stake holders using email in which developer needs to copy the information from check in, info from TFS and paste it in outlook to send across.

These action needs lot of manual intervention and are really time consuming, moreover, to map different versions (branches) in local machine takes a lot of disk as we have to download files and that consumes a lot of disk space, and most of the time the same code files need to be downloaded just to change a few lines of codes in two or three files.
This waste a lot of man hours just to do some repetitive work. 

### Solution:
This utility came into my mind as the solution of this. This will be a GUI based application that will simply push the code changes from one (source) branch to another (target) branch in
some click. User can select the from file (from source Branch) and then he will select the target version and click check-in, that's it. The code is in the target branch. No hassles of mapping the 
branches in local or virtual machines.

### Impact:-
The whole checkin process can be completed in just 10-15 seconds instead of taking 3 to 4 hours or sometimes half day (if your project is too large). It has the potential to impact the whole
development community using Team Foundation Server (TFS)
 
 #### Some features
 
*1) This utility will check in the code from source Branch file to target Branch file.*  
*2) It has the capability to associate the check-in with a real work item and mark the item as Committed-Developed.*  
*3) It has the capability to compare the code base if you are just comparing the code base. The comparison is done using the same comparing tool that visual studio uses so user need not to worry of using any new tool*  


#### Future consideration (work in progress)

*1) This utility will be able to send email (Outlook) containing information about the checkin to the configured stake holders*  
*2) At this point the utility uses static server URL, but I am working on making it more general so that it can be used by any organization*
