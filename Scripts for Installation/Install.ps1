

$SiteFolderPath = "C:\inetpub\wwwroot\FhirDemoApp2"              					# Website Folder
$SiteAppPool = "MySitePool"                  										# Application Pool Name
$SiteName = "MySite"                        										# IIS Site Name
$SiteHostName = "www.MySite.com"            										# Host Header
$SiteContentSourceFolder = "C:\Users\shanjeeva\Desktop\DemoAppSettings\Site\"       # Host Header
$SiteFolderPathBindingPath = "C:\inetpub\wwwroot\FhirDemoApp2\Site"             	# Website Folder

# C:\inetpub\wwwroot\WebApi\published\ClientApp\dist

# Create a empty folder
New-Item $SiteFolderPath -type Directory
# Set-Content $SiteFolderPath\Default.htm "<h1>Hello IIS</h1>"

# Copying the files for deployment
Copy-Item $SiteContentSourceFolder -Destination $SiteFolderPath -recurse -Force

# Create a new app pool
New-WebAppPool $SiteAppPool -force

# Create a new website
# New-WebSite -Name $SiteName -Port 80 -HostHeader $SiteName -PhysicalPath $SiteFolderPath -ApplicationPool $SiteAppPool -force
New-WebSite -Name $SiteName -Port 80 -PhysicalPath $SiteFolderPathBindingPath -ApplicationPool $SiteAppPool -force


# Setup Access rights -> Grant permisison to IIS_IUSRS
$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule("IIS_IUSRS", "FullControl", "ContainerInherit,ObjectInherit", "None", "Allow")
$acl = Get-ACL $SiteFolderPath
$acl.AddAccessRule($accessRule)
Set-ACL -Path $SiteFolderPath -ACLObject $acl

# set app pools settings 
Set-ItemProperty IIS:\AppPools\MySitePool managedRuntimeVersion ""

# Open the new site in browser
Start-Process "http://localhost/"

# Complete