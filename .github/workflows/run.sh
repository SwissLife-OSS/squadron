run() {
  local start_time=$(date '+%Y-%m-%d %H:%M:%S')
  local cpu_core=$(grep -m1 'processor' /proc/cpuinfo | awk '{print $3}')

  echo "Start Time: $start_time"
  echo "Running on CPU Core: $cpu_core"
  echo "Running: $@"

  "$@"

  local end_time=$(date '+%Y-%m-%d %H:%M:%S')

  echo "End Time: $end_time"
  echo ""
}
