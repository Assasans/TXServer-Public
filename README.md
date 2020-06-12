## Installation

In `TankiX\tankix_Data\config\clientlocal\startup\public.yml`, replace
```yaml
initUrl: <*>
stateUrl: <*>
```
with this:
```yaml
initUrl: http://127.0.0.1:8080/config/init.yml
stateUrl: http://127.0.0.1:8080/state/tankixprod_state.yml
```
`127.0.0.1` can be replaced with any other server IP address as well.

## Launch

Server starting from any IP address except `127.0.0.1` requires administrator privileges.
