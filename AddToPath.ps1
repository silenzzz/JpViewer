$old = [Environment]::GetEnvironmentVariable('Path', 'User')
$new = $args[0]

if ([String]::IsNullOrWhiteSpace($new)) {
    return "Specified path is empty"
}

if ($old.Contains($new)) {
    return "Already in PATH"
}

[Environment]::SetEnvironmentVariable('Path', $old + ';' + $new, 'User')

$updated = [Environment]::GetEnvironmentVariable('Path', 'User')

if ($updated.Contains($new)) {
    return "Appended to PATH"
} else {
    return "Error while PATH variable editing"
}