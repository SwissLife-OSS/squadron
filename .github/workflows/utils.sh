run() {
  echo -e "┌──────────────────────────────────────────────"
  echo -e "│ \033[0;34mRunning: $@\033[0m"
  echo -e "└──────────────────────────────────────────────"

  "$@"

  echo -e "\n"
}

package_version() {
  local package_version="${{ github.ref_name }}"

  if [[ "$package_version" == *"-preview"* ]]; then
    echo "It's a preview version."
  else
    echo "It's a stable version."
  fi

  echo $package_version
}

assembly_version() {
  local assembly_version=$(echo "$1" | grep -o '^[0-9]*\.[0-9]*\.[0-9]*')

  echo "${assembly_version}.0"
}
