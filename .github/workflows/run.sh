run() {
  local cpu_core=$(taskset -c -p $$ 2>&1 | awk -F': ' '{print $2}')
  local total_cores=$(nproc)

  echo -e "┌─────────────────────────────────────────────┐"
  echo -e "│ \033[0;34mRunning on CPU Core: $cpu_core of $total_cores\033[0m │"
  echo -e "├─────────────────────────────────────────────┤"
  echo -e "│ \033[0;34mRunning: $@\033[0m                               │"
  echo -e "└─────────────────────────────────────────────┘"

  "$@"

  echo -e "\n"
}
