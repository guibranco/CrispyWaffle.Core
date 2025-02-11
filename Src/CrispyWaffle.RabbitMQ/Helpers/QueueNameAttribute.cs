﻿using System;

namespace CrispyWaffle.RabbitMQ.Helpers;

/// <summary>
/// Class QueueNameAttribute. This class cannot be inherited.
/// Implements the <see cref="Attribute" />.
/// </summary>
/// <seealso cref="System.Attribute" />.
[AttributeUsage(AttributeTargets.Class)]
public sealed class QueueNameAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QueueNameAttribute" /> class.
    /// </summary>
    /// <param name="queueName">Name of the queue.</param>
    public QueueNameAttribute(string queueName) => QueueName = queueName;

    /// <summary>
    /// Gets the name of the queue.
    /// </summary>
    /// <value>The name of the queue.</value>
    public string QueueName { get; }
}
