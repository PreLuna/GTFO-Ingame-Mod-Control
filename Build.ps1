param(
 [Parameter(Mandatory=$true)][string]$GameRoot,
 [Parameter(Mandatory=$true)][string]$RuntimeRoot,
 [string]$BuildToolsRoot=(Join-Path (Split-Path $PSScriptRoot -Parent) 'GTFO-Mod-Build-and-Verification-Tools'),
 [string]$DependencyRoot='', [string]$CecilPath=''
)
$ErrorActionPreference='Stop'
& (Join-Path $BuildToolsRoot 'tools/Build-Repository.ps1') -RepositoryRoot $PSScriptRoot -GameRoot $GameRoot -RuntimeRoot $RuntimeRoot -DependencyRoot $DependencyRoot -CecilPath $CecilPath
if(!$CecilPath){$CecilPath=Join-Path $GameRoot 'BepInEx/core/Mono.Cecil.dll'}
if(-not ('Mono.Cecil.AssemblyDefinition' -as [type])){Add-Type -LiteralPath $CecilPath}
$dll=Join-Path $PSScriptRoot 'out/package/BepInEx/plugins/PreLuna.ModControl/PreLuna.ModControl.dll'
if(!(Test-Path -LiteralPath $dll -PathType Leaf)){throw ('Built DLL not found: '+$dll)}
$asm=[Mono.Cecil.AssemblyDefinition]::ReadAssembly($dll)
try{
 $archiveModule=@()
 $invalidFeatures=@()
 foreach($type in $asm.MainModule.GetTypes()){
  foreach($attribute in $type.CustomAttributes){
   if($attribute.AttributeType.FullName -eq 'TheArchive.Core.Attributes.ArchiveModule'){$archiveModule+=$type.FullName}
  }
  $base=$type.BaseType
  while($base){
   if($base.FullName -eq 'TheArchive.Core.FeaturesAPI.Feature'){
    if($type.IsAbstract -or $type.HasGenericParameters){$invalidFeatures+=$type.FullName}
    else{
     $hasDefaultCtor=$false
     foreach($method in $type.Methods){
      if($method.IsConstructor -and $method.IsPublic -and !$method.IsStatic -and $method.Parameters.Count -eq 0){$hasDefaultCtor=$true;break}
     }
     if(!$hasDefaultCtor){$invalidFeatures+=$type.FullName+' (no public parameterless constructor)'}
    }
    $hasFeatureConfig=$false
    foreach($property in $type.Properties){
     foreach($attribute in $property.CustomAttributes){
      if($attribute.AttributeType.FullName -eq 'TheArchive.Core.Attributes.Feature.Members.FeatureConfig'){
       $getter=$property.GetMethod;$setter=$property.SetMethod
       if($getter -and $setter -and $getter.IsStatic -and $setter.IsStatic -and $getter.IsPublic -and $setter.IsPublic){$hasFeatureConfig=$true}
      }
     }
    }
    if(!$hasFeatureConfig){$invalidFeatures+=$type.FullName+' (no static [FeatureConfig] property)'}
    break
   }
   if($base.FullName -eq 'System.Object'){break}
   try{$resolved=$base.Resolve()}catch{break}
   if(!$resolved){break}
   $base=$resolved.BaseType
  }
 }
 if($archiveModule.Count){throw ('[ArchiveModule] must not be used; manual registration only: '+($archiveModule -join ', '))}
 if($invalidFeatures.Count){throw ('Feature subclasses must be concrete/instantiable (TheArchive inspects every Feature type): '+($invalidFeatures -join ', '))}
}finally{$asm.Dispose()}
Write-Output ('C1 registration guard passed: no [ArchiveModule] and no invalid Feature subclass in '+[IO.Path]::GetFileName($dll))

