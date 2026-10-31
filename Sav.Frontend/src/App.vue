<template>
  <RouterView />
</template>

<style>
html,
body,
#app {
  height: 100%;
  margin: 0;
}

body {
  overflow: hidden;
  background-color: #171a21;
}

::-webkit-scrollbar {
  width: 10px;
  height: 10px;
}

::-webkit-scrollbar-track {
  background-color: #171a21;
}

::-webkit-scrollbar-thumb {
  background-color: #2a475e;
  border: 2px solid #171a21;
  border-radius: 6px;
}

::-webkit-scrollbar-thumb:hover {
  background-color: #3d6a8f;
}

::-webkit-scrollbar-corner {
  background-color: #171a21;
}
</style>