this is a example of how to create your own adapter for Nubot :)

1. you should name your adapter by the name of 'Nubot.Adapter.xxxx', in this case, its 'Nubot.Adapter.Hipchat'
2. every adapter goes with one or zero config file, which named 'xxxx.config', in this case, its 'Hipchat.config'
3. set config`s propert 'output' -> 'always copy'

ps:
	later we will consider using nuget packages for adapter and so the plugins.