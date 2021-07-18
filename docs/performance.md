# Mesure performance

```bash
start=`date +%s`; \
./artifacts/cangulo.cicd/cangulo.cicd.application --root . ; \
end=`date +%s`; \
echo Execution time was `expr $end - $start` seconds.;
```