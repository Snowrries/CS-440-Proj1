   M S B A M L   `   `   `  ���� O  KGridworld_Heuristics, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:Gridworld_HeuristicsGridworld_Heuristics  %    Gridworld_Heuristics.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     4local"clr-namespace:Gridworld_Heuristics       � Title$  
MainWindow��$
��1000��$
��1500��.��� .+��� -   $	��Map��$
��1280��$
��Left=�
��������� -   $
��Path��$
��1280��$
��Left=�
������ �� -   $��Panel��$	��200��$��Right=�.+���  �� Padding$ 10q�$��14(�Algorithm details��  �� Orientation$ 
Horizontal=�.+��� $ 10q�$��14(�Algo:�� -   $
��Algo�� �� SelectedIndex$ 0��$��10q�$��14(�$	��130������  �� Path$ Algos��� $ 
Horizontal=�.+��� $ 10q�$��14(�
Heuristic:�� -   $��	Heuristic��$ 0��$��10q�$��14(�$	��130������ $ 	heuristic��� $ 
Horizontal=�.+��� $ 10q�$��14(�	Weight:�� -   $��Weight��$��1��$��10q�$��14(�$	��120���� $ 
Horizontal=�.+��� $ 10q�$��14(�
Weight2:�� -   $��Weight2��$��1��$��10q�$��14(�$	��120���� $ 
Horizontal=�.+��� $ 10q�$��14(�f:�� $ 10q�$��14(�$	��130������ $ f��� $ 
Horizontal=�.+��� $ 10q�$��14(�g:�� $ 10q�$��14(�$	��130������ $ g��� $ 
Horizontal=�.+��� $ 10q�$��14(�h:�� $ 10q�$��14(�$	��130������ $ h��� $ 
Horizontal=�.+��� $ 10q�$��14(�
Runtime:�� -   $��runtime��$ 10q�$��14(�$	��110������ $ Runtime��� $ 
Horizontal=�.+��� $ 10q�$��14(�
PathLen:�� -	   $��
pathlength��$ 10q�$��14(�$	��110������ $ PathLen��� $ 
Horizontal=�.+��� $ 10q�$��14(�	Expanded:�� -
   $��Expanded��$ 10q�$��14(�$	��110������ $ Expanded��� $ 10q�$��14(�Start End Pairs:�� -   $��StartEndPairs��$��10q�$��14(�$	��130��$ 0������ *
PairList+�� $ 10q�$��14(�Map Selection:�� -   $��	MapSelect��$��10q�$��14(�$	��130������ $ MapList��� $  5q��� -   $��Generate��$��10q�.��Generate new worlds�� $  3q��� -   $��	Calculate��$��10q�.��Calculate Path�� $ 10q�$��14(�	Status:�� -   $��status1��$ 10q�$��10(�...�� -   $��status2��$ 10q�$��10(�...�� -   $��status3��$ 10q�$��10(�...