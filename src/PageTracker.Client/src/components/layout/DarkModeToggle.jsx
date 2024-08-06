import { Button, useMantineColorScheme, useComputedColorScheme } from '@mantine/core'

function DarkModeToggle() {
  const { setColorScheme } = useMantineColorScheme();
  const computedColorScheme = useComputedColorScheme('light');

  const toggleColorScheme = () => {
    setColorScheme(computedColorScheme === 'dark' ? 'light' : 'dark')
  };

  return (
    <Button size="compact-xs" color="warm-red.3" onClick={toggleColorScheme}>
      {computedColorScheme === 'dark' ? <span className="fi fi-rs-sun"></span> : <span className="fi fi-rs-moon-stars"></span>}
    </Button>
  );
}

export default DarkModeToggle;