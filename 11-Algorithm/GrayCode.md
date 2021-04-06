# **`Gray Code`** Basics

**`Gray Codes`** are used in encoders, altimeters, and Karnaugh maps due to their **error detection** and **unit-distant** properties.

## What are **`Gray Codes`**?
A **`Gray Code`** represents numbers using a **binary encoding scheme** that groups a sequence of bits so that **only one bit in the group changes from the number before and after**. 

> It is named for **Bell Labs** researcher **Frank Gray**, who described it in his 1947 patent submittal on Pulse Code Communication. He did not call it a **`Gray Code`**, but noted there was no name associated with the novel code and referred to it as a **Binary Reflected Code** for the way he determined the groupings and number representations. When the patent was granted in 1953 others began to refer to the encoding scheme as the **`Gray Code`**. The encoding was used in some applications prior to Gray's patent, but Frank Gray was the first to document the code and how to develop it using the 'reflection' method in a patent application.

A **`Gray Code`** is **not weighted**, the columns of bits do not reflect an implicit base weight as the Binary number system does. In the Binary number system, the least significant bit (right-most) column is weighted as 2^0 (1); the second column, 2^1 (2); the third 2^2 (4), and so on, each column representing the base raised to a power. The final value is determined by multiplying the bit by the column weight and adding the column results, so in Binary the 4-bit number '0011' represents 1*2 + 1*1 = 3. 

Columns in the **`Gray Code`** are positional but not weighted and since a **`Gray Code`** is a numeric representation of a **cyclic encoding scheme**, where it will roll over and repeat, **it isn't suited for mathematical operations**. **`Gray Code`** sequences have to be converted to Binary or Binary Coded Decimal (BCD) if they are used in mathematical computations or for displays.

A member of unit-distant, minimal-change codes, where only one bit of a sequence changes as the number count progresses, **`Gray Codes`** provide more flexibility with respect to misalignment and synchronization because they limit the maximum read error to one unit. This property also makes them useful in error detection schemes. Better than a parity check, communication systems use **`Gray Codes`** to detect unexpected changes in data. If the bits in a number are summed, the sum of the next number should only change by one with the sum alternating even and odd.

A comparison of the first ten numbers in Decimal, Binary and **`Gray Code`** is shown in Table 1.

|Decimal (base 10)  |Binary (base 2)  |Binary-Reflected (no base)|
|-------------------|-----------------|--------------------------|
| 0                 | 0000            | 0000                     |
| 1                 | 0001            | 0001                     |
| 2                 | 0010            | 0011                     |
| 3                 | 0011            | 0010                     |
| 4                 | 0100            | 0110                     |
| 5                 | 0101            | 0111                     |
| 6                 | 0110            | 0101                     |
| 7                 | 0111            | 0100                     |
| 8                 | 1000            | 1100                     |
| 9                 | 1001            | 1101                     |
| 10                | 1010            | 1111                     |

Table 1. Decimal, Binary, **`Gray Code`** Numbers

## **A Patented Code*

Frank Gray's original patent submittal introduced the code with respect to a coding mask for a cathode beam sweep to eliminate errors due to the sweep mechanics. In the patent, Gray referred to the Binary number system as conventional binary, to differentiate his coding mask from those currently employed at the time. The pulse codes were the result of sampling message amplitudes and grouping the resulting on/off pulses into a series of binary groups. Gray referred to the code as a binary n-digit code because the groups could be bits of 4, 5, 7 or any number n. With different numbers of bits in a group and permutations of the bits, the code could produce various implementations.

The patent detailed the problem using a conventional binary notation for the mask. In the Binary system, going from the number 7 to 8 (a 1 unit increment) required all the bits representing the number to change: 0111 became 1000. It's nice to assume every bit in the group changes state at the same time, but with mechanical based systems that may not be the case depending on the individual mechanical responses and the timing of the read cycles. A read operation while the bits are changing may result in bad data. For the case where 7 increments to 8, the changing bits could reflect a number from 1111 to 0000 depending on the individual bit transitions and when the bits are read.

While the patent mentions the problem having all digits changing at once could lead to errors beyond the inherent errors with the cathode beam control, this same situation could cause errors when a time lag or mechanical response might cause incorrect readings. For example, if mechanical switches are used there may be issues with debouncing or actual timing in settling to 0 or 1. Simply allowing enough time to be certain all the mechanics have settled may seriously affect timing and may not be possible depending on system requirements.

With a **`Gray Code`**, where only one of the bits change for each transition, the chance for such an error is reduced. Going from 7 to 8 in the Binary-Reflected **`Gray Code`** (BRGC), the bit sequence changes from 0100 to 1100. At any point, the number read is either 7 or 8, as the rest of the bits stay the same. The possibility of incorrect values being read due to multiple bit changes is minimized; worst case, the value may be off by one.

Any code with this unit change property is referred to as a **`Gray Code`**. The concept existed before Gray's patent. Émile Baudot, the French engineer responsible for the Baudot Code, used it in a telegraph demonstration in 1878 but Gray's patent documented the steps for determining a specific coding order.

## Devising the Code

The Reflected Binary Code specified by Frank Gray in his patent application is determined using the following steps:

Starting with bits in the first column:

0
1

Take the reflection, as if a mirror were held up to the column:

0
1______mirror
1
0

This results in a column with 4 entries, but the first and last are the same, as are the middle ones, so another column and alternate bits are added:

00
01
11
10

Then reflect that:

00
01
11
10______mirror
10
11
01
00

and add another column with alternate bits:

000
001
011
010
110
111
101
100

and it continues.

A **`Gray Code`** is useful when rapidly changing values could result in errors due to hardware and interfacing constraints. Many rotary mechanical and optical encoders offer **`Gray Code`** outputs, such as Electrocam, Mouser, and Digi-Key.

A representative **`Gray Code`** encoding wheel is shown below.  Each position on the disk corresponds to a binary sequence, with only one bit changing. Ordinary binary is in black font while **`Gray Code`** is in red.

![Class hierarchy](assets\Gray_Code_encoding_wheel_resize.png "Gray Code") <br/>
 

## Uses

**`Gray Codes`** have gone beyond the encoding mask documented in the patent; **`Gray Codes`** are now incorporated into systems where one-bit detection is useful.

In aircraft, where altimeters are normally mechanical, an encoding disk synced to the dials may produce a type of **`Gray Code`** output (Gillham Code) to send to the transponder for processing. This specialized code reflects a one bit change for each 100 foot increment allowing the altitude to be tracked.

In Karnaugh maps (K-maps), a graphical tool to used to simplify digital circuits and identify potential race conditions, variables are arranged in **`Gray Code`** order. K-maps were developed in 1954 by Maurice Karnaugh (another researcher at Bell Labs) as a refinement of Edward Veitch's Veitch diagrams. K-maps were routinely used by digital designers before computers and automated design tools were available. For more information on Karnaugh Maps, check out our textbook page on Karnaugh maps or our article on the Karnaugh map Boolean algebraic simplification technique.

## Other **`Gray Codes`**

The term **`Gray Code`** has expanded to include any unit-distant coding scheme. In addition to those mentioned above, other Gray codes developed for specific situations include:

n-ary **`Gray Code`**, where non-Boolean values are included, like sequences of 1, 2, 3.

Two dimensional (n,k)-**`Gray Codes`**, used for error correction.

Balanced **`Gray Codes`**, where all transition counts are equal (n must be a power of 2).

Beckett–Gray Code, named for the Irish playwright Samuel Beckett, who was interested in the order performers entered and exited the stage.

In Graph Theory, snake-in-the-box codes (snakes) and coil-in-the-box codes (coils) are referred to as **`Gray Codes`**, because they detect single bit coding errors.

In rotary systems, single-track **`Gray Codes`** (STGC) are used to sense contacts with rotary tracks.

 

## Summary

Although originally developed for a specific application, **`Gray Codes`**, coding schemes where the bits representing a number only differ by one bit between the number before and after, have found uses in rotary and optical encoders, error detection and Karnaugh maps. 

With only one bit changing state as numbers progress, mechanical and timing issues that may cause read errors are minimized. Many rotary encoders and data acquisition systems offer **`Gray Code`** outputs.